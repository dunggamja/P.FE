using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
  public sealed class DynamicAABBTree 
  {
    class Node : IPoolObject
    {
      public Int64 id; // 객체 ID (리프에서만 유효)
      public int   height = 0;
      public AABB  box;
      public Node  left, right, parent;
      public bool  IsLeaf => left == null && right == null;

      public void Reset()
      {
        box    = default;
        left   = null;
        right  = null;
        parent = null;
        id     = 0;
      }


      // 둘레값: node 위치 정할때 사용하는 것 같음... 왜 하는지는 아직 모르겠다. 
     
    }

    Node                    m_root     = null;
    Dictionary<Int64, Node> m_node_map = new(20);

    public void QueryAABB(AABB _query_box, ref List<Int64> _results) 
    {
      if (m_root == null) 
        return;

      var stack = ListPool<Node>.Acquire();
      stack.Add(m_root);       
      // var stack = new Stack<Node>(); stack.Push(root);

      while (stack.Count > 0) 
      {
        var n = stack[stack.Count - 1];
        stack.RemoveAt(stack.Count - 1);

        if (!n.box.Intersects(_query_box)) 
          continue;

        if (n.IsLeaf) 
        {
          _results.Add(n.id);
        }
        else 
        { 
          stack.Add(n.left); 
          stack.Add(n.right); 
        }
      }

      ListPool<Node>.Return(ref stack);
    }

    public void Insert(Int64 _id, AABB _box)
    {
      if (m_node_map.TryGetValue(_id, out var node))
      {
        // 갱신 처리.
        Update(_id, _box);
        return;
      }
      
      node     = ObjectPool<Node>.Acquire();
      node.id  = _id;
      node.box = _box;

      m_node_map.Add(_id, node);
      InsertLeaf(node);
    }

    public void Remove(Int64 _id)
    {
      if (m_node_map.TryGetValue(_id, out var node) == false)
        return;

      RemoveLeaf(node);
      m_node_map.Remove(_id);

      ObjectPool<Node>.Return(ref node);
      return;
    }

    void Update(Int64 _id, AABB _box)
    {
      // 존재하지 않으면 실패.
      if (m_node_map.TryGetValue(_id, out var node) == false)
        return;

      var fat_box = AABB.Fatten(_box, 0.5f);

      // 이미 포함되어 있으면 실패.
      if (node.box.Contains(fat_box))
        return;

      // 삭제.
      RemoveLeaf(node);

      // 크기 변경.
      node.box = fat_box;

      // 삽입.
      InsertLeaf(node);
    }

    void InsertLeaf(Node _node)
    {
      if (m_root == null)
      {
        m_root = _node;
        _node.parent = null;
        return;
      }

      // 형제 노드 선택. (표면적이 가장 작은 친구를 골라본다.)
      Node sibling    = ChooseBestSibling(_node.box);
      Node parent_old = sibling.parent;

      // 새로운 부모노드를 생성한다.
      Node parent_new   = ObjectPool<Node>.Acquire();
      parent_new.parent = parent_old;
      parent_new.left   = sibling;
      parent_new.right  = _node;      
      parent_new.box    = AABB.Combine(sibling.box, _node.box);
      parent_new.height = sibling.height + 1;

      // 새 부모노드에 연결.
      sibling.parent = parent_new;
      _node.parent   = parent_new;

      // 기존 부모노드에 연결.
      if (parent_old != null)
      {
        if (parent_old.left == sibling) parent_old.left  = parent_new;
        else                            parent_old.right = parent_new;
      }
      else
      {
        m_root = parent_new;
      }
      // 새 부모노드 상향?
      FixUpward(parent_new);
    }

    void RemoveLeaf(Node _node)
    {
      if (_node == m_root)
      {
        m_root = null;
        return;
      }

      Node parent  = _node.parent;
      Node grand   = parent.parent;
      Node sibling = (parent.left == _node) ? parent.right : parent.left;

      if (grand != null)
      {
        // 할배 노드의 자식노드 셋팅.
        if (grand.left == parent) grand.left  = sibling;
        else                      grand.right = sibling;

        // 형제 노드의 새 부모노드 셋팅.
        sibling.parent = grand;

        // 새 부모노드 상향 처리.
        FixUpward(grand);
      }
      else
      {
        m_root         = sibling;
        sibling.parent = null;
      }

      // 부모노드는 더 이상 사용하지 않으니 반환.
      parent.left = parent.right = parent.parent = null;
      ObjectPool<Node>.Return(ref parent);
    }

    Node ChooseBestSibling(AABB _box)
    {
      var node = m_root;
      if (node == null)
        return null;

      while(node.IsLeaf == false)
      {
        var left  = node.left;
        var right = node.right;

        // 둘레값
        var perimeter         = node.box.Perimeter();
      
        var combine_box       = AABB.Combine(node.box, _box);

        var combine_perimeter = combine_box.Perimeter();

        // 둘레값 계산
        // (3가지 중 둘레값이 가장 적게 증가하는 비용을 택해봅시다.
        var cost_parent = 2f * combine_perimeter;
        var cost_left   = 2f * AABB.Combine(left.box,  _box).Perimeter() - perimeter;
        var cost_right  = 2f * AABB.Combine(right.box, _box).Perimeter() - perimeter;

        // 부모가 가장 싼 경우
        if (cost_parent < cost_left && cost_parent < cost_right)
          break;

        // 좌/우 중 더 싼 쪽으로 하강.
        node = (cost_left < cost_right) ? left : right;
      }

      return node;
    }


    void FixUpward(Node _node)
    {
      while (_node != null)
      {
        //TODO: 비용 개선을 위해서는. 리밸런싱 기능을 추가하면 좋다고 한다.
        var left  = _node.left;
        var right = _node.right;


        // 높이값 갱신.
        _node.height = Math.Max(left?.height ?? -1, right?.height ?? -1) + 1;

        // 박스 갱신.
        if (left != null && right != null)
          _node.box = AABB.Combine(left.box, right.box);

        // 상향.
        _node = _node.parent;
      }

    }
    
  }
  }