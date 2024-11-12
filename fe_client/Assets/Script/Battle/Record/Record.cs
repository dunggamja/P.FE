using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumRecordType
{
    None,

    Record_Command,   // 명령 기록
    Record_Data,      // 데이터 기록    
}

public interface IRecord
{
    EnumRecordType RecordType { get; }
    Int64          EntityID   { get; } 
    Int64          TimeID     { get; } // TODO: BatttleSystem Turn에서 관리해보자...
}


public interface IRecordData_Owner
{
    IRecord       CreateRecordData();

    IRecordObject CreateByRecordData(IRecord _record_data); // static 함수로.
}

public interface IRecordCommand_Owner
{
    void     UndoRecordCommand(IRecord _record_data); 
}


public class Record<T> where T : IRecordData
{
    List<T> m_list_record = new(5);
 

    public T GetRecordLast(int _index)
    {
        if (m_list_record.Count == 0 || _index < 0 || m_list_record.Count <= _index)
            return null;

        return m_list_record[m_list_record.Count - 1 - _index]; 
    }

    public void PushRecord(T _record)
    {
        m_list_record.Add(_record);
    }  

    public void PopRecord()
    {
        if (m_list_record.Count == 0)
            return;

        m_list_record.RemoveAt(m_list_record.Count - 1);
    }

    public void PopRecordCount(int _count)
    {
        if (m_list_record.Count == 0 || _count <= 0)
            return;

        _count = Mathf.Min(m_list_record.Count, _count);

        m_list_record.RemoveRange(m_list_record.Count - _count, _count);
    }
   
}
