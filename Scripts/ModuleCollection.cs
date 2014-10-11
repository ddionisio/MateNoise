using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Reflection;
using System.ComponentModel;

namespace M8.Noise {
    public class ModuleCollection {
        public class Info {
            public string name;
            public string type;
            public string[] parameters;
            public string[] sourceModules;
        }

        public Module.ModuleBase this[string name] {
            get {
                Module.ModuleBase ret;
                mModules.TryGetValue(name, out ret);
                return ret;
            }
        }
                
        public ModuleCollection(Info[] infos) {
            Module.ModuleBase[] modules = new Module.ModuleBase[infos.Length];

            for(int i = 0; i < infos.Length; i++) {
                Info inf = infos[i];

                //create module based on type
                Utils.Instance constructor = Utils.Generate(inf.type);
                if(constructor != null) {
                    Module.ModuleBase module = constructor() as Module.ModuleBase;

                    //fill in fields based on parameters, each param is "field=value"
                    Type t = module.GetType();

                    char[] delims = new char[] { '=' };

                    if(inf.parameters != null) {
                        for(int p = 0; p < inf.parameters.Length; p++) {
                            string parm = inf.parameters[p];
                            string[] pair = parm.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                            pair[0].Trim();
                            pair[1].Trim();

                            FieldInfo field = t.GetField(pair[0]);
                            if(field != null) {
                                TypeConverter converter = TypeDescriptor.GetConverter(field.FieldType);
                                if(converter != null)
                                    field.SetValue(module, converter.ConvertFromString(pair[1]));
                            }
                            else {
                                //try property
                                PropertyInfo prop = t.GetProperty(pair[0]);
                                if(prop != null) {
                                    TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);
                                    if(converter != null)
                                        prop.SetValue(module, converter.ConvertFromString(pair[1]), null);
                                }
                                else {
                                    Debug.LogWarning("Unknown parameter: "+pair[0]+" for type: "+t.ToString());
                                }
                            }
                        }
                    }

                    modules[i] = module;
                }
                else
                    Debug.LogWarning("Unknown type: "+inf.type);
            }

            //add to collection
            mModules = new Dictionary<string, Module.ModuleBase>(modules.Length);
            for(int i = 0; i < modules.Length; i++) {
                Module.ModuleBase module = modules[i];
                if(module != null) {
                    mModules.Add(infos[i].name, module);
                }
            }

            //setup source modules
            for(int i = 0; i < modules.Length; i++) {
                Info inf = infos[i];
                if(inf.sourceModules != null) {
                    Module.ModuleBase module = modules[i];
                    if(module != null) {
                        if(module is Module.Sum)
                            ((Module.Sum)module).sourceModuleCount = inf.sourceModules.Length;

                        int sourceCount = Mathf.Min(inf.sourceModules.Length, module.sourceModuleCount);
                        for(int j = 0; j < sourceCount; j++) {
                            Module.ModuleBase src;
                            if(mModules.TryGetValue(inf.sourceModules[j], out src))
                                module[j] = src;
                        }
                    }
                }
            }
        }

        private Dictionary<string, Module.ModuleBase> mModules;
    }
}