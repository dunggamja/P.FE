//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SystemManager<_TSystem, _TParam>  :
//    ISystemManager
//    where _TSystem : System<_TParam>
//    where _TParam  : ISystemParam
//{
//    protected Dictionary<int, _TSystem> m_repository = new Dictionary<int, _TSystem>();


//    public _TParam   Param { get; protected set; }
//    public EnumState State { get; protected set; }

//    public bool IsProgress => State == EnumState.Progress;
//    public bool IsFinished => State == EnumState.Finished;

//    public void Update()
//    {
//        if (State != EnumState.Progress)
//        {
//            //State = EnumState.Init;
//            State = EnumState.Progress;
//            OnEnter();
//        }

//        if (OnUpdate())
//        {
//            State = EnumState.Finished;
//        }

//        if (State != EnumState.Progress)
//        {
//            OnExit();
//        }
//    }

//    public bool AddSystem(_TSystem _system)
//    {
//        if (_system == null)
//            return false;

//        if (m_repository.TryGetValue((int)_system.SystemType, out var system))
//        {
//            Debug.LogError($"Can't Add System, {_system.SystemType.ToString()} in SystemManager[{GetType().ToString()}]");
//            return false;
//        }

//        m_repository.Add((int)_system.SystemType, _system);
//        return true;
//    }

//    public ISystem GetSystem(EnumSystem _system_type)
//    {
//        if (m_repository.TryGetValue((int)_system_type, out var system))
//            return system;

//        Debug.LogError($"Can't Find System, {_system_type.ToString()} in SystemManager[{GetType().ToString()}]");
//        return null;
//    }

//    public EnumState UpdateSystem(EnumSystem _system_type, _TParam _param)
//    {
//        var system = GetSystem(_system_type) as _TSystem; 
//        if (system != null)
//            return system.Update(_param);

//        return EnumState.None;
//    }

//    public EnumState GetSystemState(EnumSystem _system_type)
//    {
//        var system = GetSystem(_system_type);
//        if (system != null)
//            return system.State;

//        return EnumState.None;
//    }

//    public bool IsSystemFinished(EnumSystem _system_type)
//    {
//        return GetSystemState(_system_type) == EnumState.Finished;
//    }

//}
