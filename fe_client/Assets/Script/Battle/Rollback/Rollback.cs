using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class Rollback
    {
        // EnumRecordType RecordType { get; }
        // Int64          EntityID   { get; } 

        public Int64  ID   { get; private set; } = 0; 
        public Int32  Turn { get; private set; } = 0;

        public abstract void Undo();
        // public abstract void Redo();

        protected Rollback(int _turn)
        {
            ID   = Util.GenerateID();
            Turn = _turn;
        }
    }




    public class RollbackManager : Singleton<RollbackManager>
    {
        List<Rollback>  m_list_rollback = new(10);

        int MaxRollbackCount => 1000;
    

        public void Reset()
        {
            m_list_rollback.Clear();
        }
        
        public int   RollbackCount() => m_list_rollback.Count;

        public Rollback Peek(int _index = 0)
        {
            if (m_list_rollback.Count == 0 || _index < 0 || m_list_rollback.Count <= _index)
                return null;

            return m_list_rollback[m_list_rollback.Count - 1 - _index]; 
        }

        public void Push(Rollback _record)
        {
            m_list_rollback.Add(_record);

            
            ClearOldData();
        }  

        public void Pop()
        {
            if (m_list_rollback.Count == 0)
                return;

            m_list_rollback.RemoveAt(m_list_rollback.Count - 1);
        }   

        public void Rollback(Int64 _id)
        {
            // �ѹ��� �����մϴ�. 
            while(0 < RollbackCount())
            {                
                var rollback = Peek();

                // �Է¹��� id�� ������ ������ �ѹ��� �������� �ʽ��ϴ�.
                if (rollback == null || rollback.ID < _id)
                    break;

                // �ѹ� ó��
                rollback.Undo();

                // �ѹ��� Record ����.
                Pop();
            }
        }

        private void ClearOldData()
        {
            // 
            if (m_list_rollback.Count <= MaxRollbackCount)
                return;

            var old_data = m_list_rollback[0];
            var new_data = m_list_rollback[m_list_rollback.Count - 1];

            // null�� �����͵��� ������ ��� ����.
            if (old_data == null || new_data == null)
            {                
                m_list_rollback.RemoveAll(e => e == null);
                return;
            }

           // Turn ������ �����.
            if (old_data.Turn == new_data.Turn)
                return;

            var delete_turn    = old_data.Turn;
            int delete_count   = 0;
            int overflow_count = m_list_rollback.Count - MaxRollbackCount;

            for (int i = 0; i < m_list_rollback.Count; ++i, ++delete_count)            
            {
                var data = m_list_rollback[i];
                if (data == null)
                {
                    continue;
                }
                
                // Turn ������ �����.
                if (data.Turn == delete_turn)
                {
                    continue;
                }

                // �ʿ��� ��ŭ �������� break   
                if (overflow_count <= delete_count)
                {
                    break;
                }

                // ���� Turn�� ����� ����...
                if (data.Turn == new_data.Turn)
                {
                    break;
                }

                delete_turn = data.Turn;
            }

            // ������ �����͵��� ����ô�.
            m_list_rollback.RemoveRange(0, delete_count);
        }

    }
}
