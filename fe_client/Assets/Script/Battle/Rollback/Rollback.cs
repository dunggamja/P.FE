using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class IRollback
    {
        // EnumRecordType RecordType { get; }
        // Int64          EntityID   { get; } 
        public Int64  TimeStamp  { get; protected set; } = 0; // TODO: BatttleSystem Turn과 연관이 되어야 할듯.

        public abstract void     Rollback();
    }




    public class RollbackManager : Singleton<RollbackManager>
    {
        List<IRollback>  m_list_rollback = new(10);


        public void Reset()
        {
            m_list_rollback.Clear();
        }
        
        public int   RollbackCount() => m_list_rollback.Count;

        public IRollback Peek(int _index = 0)
        {
            if (m_list_rollback.Count == 0 || _index < 0 || m_list_rollback.Count <= _index)
                return null;

            return m_list_rollback[m_list_rollback.Count - 1 - _index]; 
        }

        public void Push(IRollback _record)
        {
            m_list_rollback.Add(_record);
        }  

        public void Pop()
        {
            if (m_list_rollback.Count == 0)
                return;

            m_list_rollback.RemoveAt(m_list_rollback.Count - 1);
        }   

        public void Rollback(Int64 _time_stamp)
        {
            // 입력받은 시간 까지 롤백을 진행합니다. 
            while(0 < RollbackCount())
            {
                // 시간 체크.
                var rollback = Peek();
                if (rollback == null || rollback.TimeStamp < _time_stamp)
                    break;

                // 롤백 처리
                rollback.Rollback();

                // 롤백한 Record 제거.
                Pop();
            }
        }

    }
}
