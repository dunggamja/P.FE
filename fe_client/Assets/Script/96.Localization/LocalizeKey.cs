using UnityEngine;

public struct LocalizeKey
{
    public readonly string Table;
    public readonly string Key;

    private LocalizeKey(string table, string key)
    {
        Table = table;
        Key   = key;
    }

    public static LocalizeKey Create(string table, string key)
    {
        return new LocalizeKey(table, key);
    }

  

    

    // // �� ��� �񱳸� ���� Equals�� GetHashCode �������̵�
    // public override bool Equals(object obj)
    // {
    //     if (obj is LocalizeKey other)
    //     {
    //         return Table == other.Table && Key == other.Key;
    //     }
    //     return false;
    // }

    // public override int GetHashCode()
    // {
    //     unchecked
    //     {
    //         int hash = 17;
    //         hash = hash * 23 + (Table?.GetHashCode() ?? 0);
    //         hash = hash * 23 + (Key?.GetHashCode() ?? 0);
    //         return hash;
    //     }
    // }

    // // ������� ���� ToString �������̵�
    // public override string ToString()
    // {
    //     return $"LocalizeData(Table: {Table}, Key: {Key})";
    // }
}
