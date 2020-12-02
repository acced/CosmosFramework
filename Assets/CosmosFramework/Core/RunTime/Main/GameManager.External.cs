﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    internal static partial class GameManager
    {
        /// <summary>
        /// 自定义模块容器；
        /// </summary>
        static Dictionary<Type, Module> externalModuleDict = new Dictionary<Type, Module>();
        /// <summary>
        /// 线程安全；
        /// 获取自定义模块；
        /// 需要从Module类派生;
        /// 此类模块不由CF框架生成，由用户自定义
        /// </summary>
        /// <typeparam name="TModule">实现模块功能的类对象</typeparam>
        /// <returns>获取的模块</returns>
        public static TModule ExternalModule<TModule>()
            where TModule : Module, new()
        {
            Type type = typeof(TModule);
            Module module = default;
            var result = externalModuleDict.TryGetValue(type, out module);
            if (result)
            {
                return module as TModule;
            }
            else
                return default(TModule);
        }
        /// <summary>
        /// 初始化自定义模块
        /// </summary>
        /// <param name="assembly">模块所在程序集</param>
        public static void InitExternalModule(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            var moduleType = typeof(Module);
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].GetCustomAttribute<ExternalModuleAttribute>() != null && moduleType.IsAssignableFrom(types[i]))
                {
                    try
                    {
                        var module = Utility.Assembly.GetTypeInstance(types[i]) as Module;
                        var result = externalModuleDict.TryAdd(types[i], module);
                        if (result)
                        {
                            module.OnInitialization();
                            Utility.Debug.LogInfo($"Custome Module :{module} is OnInitialization");
                            GameManager.RefreshHandler+= module.OnRefresh;
                        }
                    }
                    catch
                    {
                        Utility.Debug.LogError($"Custome module create instance fail:{types[i]}");
                    }
                }
            }
            ActiveExternalModule();
        }
        static void ActiveExternalModule()
        {
            foreach (var module in externalModuleDict.Values)
            {
                module.OnActive();
            }
            PrepareExternalModule();
        }
        static void PrepareExternalModule()
        {
            foreach (var module in externalModuleDict.Values)
            {
                module.OnPreparatory();
            }
        }
    }
}
