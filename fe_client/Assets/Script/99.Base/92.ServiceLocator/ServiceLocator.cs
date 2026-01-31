using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;



public static class ServiceLocator
{
   public const int GLOBAL = 1;
}


public static class ServiceLocator<T> where T : class, new()
{
   private static Dictionary<int, T> m_repository = new ();


   public static T Get(int _type)
   {
      if (m_repository.TryGetValue(_type, out var service))
         return service;

      return null;
   }

   public static bool TryGet(int _type, out T _service)
   {
      return m_repository.TryGetValue(_type, out _service);
   }

   public static void Register(int _type, T _service)
   {
      m_repository[_type] = _service;
   }





   
}