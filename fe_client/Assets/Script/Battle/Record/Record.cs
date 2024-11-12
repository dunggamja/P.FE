using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecordData
{
    Int64 EntityID { get; } 
    Int64 TimeID   { get; } // TODO: BatttleSystem Turn에서 관리해보자...
}

public interface IRecordObject
{
    IRecordData   CreateRecordData();

    IRecordObject CreateByRecordData(IRecordData _record_data);
}

public class Record<T> where T : IRecordData
{
    List<T> m_list_record = new(5);

    public void AddRecord(T _record)
    {
        m_list_record.Add(_record);
    }   

    public T GetRecordLast(int _index)
    {
        if (m_list_record.Count == 0 || _index < 0 || m_list_record.Count <= _index)
            return null;

        return m_list_record[m_list_record.Count - 1 - _index]; 
    }
   
}
