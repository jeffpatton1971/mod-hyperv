namespace mod_hyperv
{
    using System;
    using System.Collections.Generic;
    using System.Management;
    class Functions
    {
        public enum ResourceType
        {
            //  
            // http://msdn.microsoft.com/en-us/library/cc136903(v=vs.85).aspx
            //
            Other = 1,
            ComputerSystem = 2,
            Processor = 3,
            Memory = 4,
            IDEController = 5,
            ParallelSCSIHBA = 6,
            FCHBA = 7,
            iSCSIHBA = 8,
            IBHCA = 9,
            EthernetAdapter = 10,
            OtherNetworkAdapter = 11,
            IOSlot = 12,
            IODevice = 13,
            FloppyDrive = 14,
            CDDrive = 15,
            DVDDrive = 16,
            SerialPort = 17,
            ParallelPort = 18,
            USBController = 19,
            GraphicsController = 20,
            StorageExtent = 21,
            Disk = 22,
            Tape = 23,
            OtherStorageDevice = 24,
            FirewireController = 25,
            PartitionableUnit = 26,
            BasePartitionableUnit = 27,
            PowerSupply = 28,
            CoolingDevice = 29
            //
            // DMTF Reserved had no value listed
            // VendorReserved had several values listed
            //
        }
        public sealed class wmiClass
        {
            private readonly string Name;

            public static readonly wmiClass Msvm_ComputerSystem = new wmiClass("Msvm_ComputerSystem");
            public static readonly wmiClass Msvm_PlannedComputerSystem = new wmiClass("Msvm_PlannedComputerSystem");
            public static readonly wmiClass Msvm_SettingsDefineState = new wmiClass("Msvm_SettingsDefineState");
            public static readonly wmiClass Msvm_VirtualSystemManagementService = new wmiClass("Msvm_VirtualSystemManagementService");
            public static readonly wmiClass Msvm_VirtualSystemSettingData = new wmiClass("Msvm_VirtualSystemSettingData");
            public static readonly wmiClass Msvm_VirtualSystemManagementServiceSettingData = new wmiClass("Msvm_VirtualSystemManagementServiceSettingData");
            public static readonly wmiClass Msvm_VirtualSystemSnapshotService = new wmiClass("Msvm_VirtualSystemSnapshotService");
            public static readonly wmiClass CIM_ResourcePool = new wmiClass("CIM_ResourcePool");
            public static readonly wmiClass Msvm_StorageAllocationSettingData = new wmiClass("Msvm_StorageAllocationSettingData");
            public static readonly wmiClass Msvm_VirtualSystemSettingDataComponent = new wmiClass("Msvm_VirtualSystemSettingDataComponent");
            public static readonly wmiClass Msvm_VirtualHardDiskSettingData = new wmiClass("Msvm_VirtualHardDiskSettingData");
            public static readonly wmiClass Msvm_ImageManagementService = new wmiClass("Msvm_ImageManagementService");
            private wmiClass(String Name)
            {
                this.Name = Name;
            }
            public override string ToString()
            {
                return Name;
            }
        }
        //
        // Publicly available functions
        //
        public static ManagementObject getVM(string Name, ManagementScope Scope)
        {
            return getManagementObject(Name, wmiClass.Msvm_ComputerSystem.ToString(), Scope);
        }
        public static ManagementObject getVMSettingData(ManagementObject vMachine, ManagementScope Scope)
        {
            return getSettingData(wmiClass.Msvm_VirtualSystemSettingData.ToString(), wmiClass.Msvm_SettingsDefineState.ToString(), vMachine, Scope);
        }
        public static ManagementObject getVMHost(ManagementScope Scope)
        {
            return getManagementObject(Environment.MachineName, wmiClass.Msvm_ComputerSystem.ToString(), Scope);
        }
        public static ManagementObject getVMHost(string HostName, ManagementScope Scope)
        {
            return getManagementObject(HostName, wmiClass.Msvm_ComputerSystem.ToString(), Scope);
        }
        public static ManagementObject getPlannedVirtualMachine(string Name, ManagementScope Scope)
        {
            return getManagementObject(Name, wmiClass.Msvm_PlannedComputerSystem.ToString(), Scope);
        }
        public static ManagementObject getVMManagementService(ManagementScope Scope)
        {
            return getManagementObject(wmiClass.Msvm_VirtualSystemManagementService.ToString(), Scope);
        }
        public static ManagementObject getVMManagementServiceSetting(ManagementScope Scope)
        {
            return getSettingData(wmiClass.Msvm_VirtualSystemManagementServiceSettingData.ToString(), Scope);
        }
        public static ManagementObject getVMSnapshotService(ManagementScope Scope)
        {
            return getManagementObject(wmiClass.Msvm_VirtualSystemSnapshotService.ToString(), Scope);
        }
        public static ManagementObject getResourcePool(ResourceType ResourceType, string ResourceSubtype, string PoolId, ManagementScope Scope)
        {
            return getManagementObject(wmiClass.CIM_ResourcePool.ToString(), ResourceType, ResourceSubtype, PoolId, Scope);
        }
        public static ManagementObjectCollection getResourcePools(ResourceType ResourceType, string ResourceSubtype, string PoolId, ManagementScope Scope)
        {
            var myType = (int)ResourceType;
            Console.WriteLine("ResourceType : " + ResourceType.ToString());
            return getManagementObjects(wmiClass.CIM_ResourcePool.ToString(), ResourceType, ResourceSubtype, PoolId, Scope);
        }
        public static ManagementObject[] getVhdSettings(ManagementObject virtualMachine, ManagementScope Scope)
        {
            using (ManagementObject mObject = getSettingData(wmiClass.Msvm_VirtualSystemSettingData.ToString(), wmiClass.Msvm_SettingsDefineState.ToString(), virtualMachine, Scope))
            {
                return getVHDSettings(mObject);
            }
        }
        public static ManagementObject getVhdSettingData(ManagementObject virtualMachine, ManagementScope Scope)
        {
            return getSettingData(wmiClass.Msvm_VirtualHardDiskSettingData.ToString(), Scope);
        }
        public static ManagementObject getImageManagementService(ManagementScope Scope)
        {
            return getManagementObject(wmiClass.Msvm_ImageManagementService.ToString(), Scope);
        }
        //
        // Private functions used internally
        //
        private static ManagementObjectCollection getManagementObjects(string ClassName, ResourceType ResourceType, string ResourceSubtype, string Id, ManagementScope mScope)
        {
            string strQuery = null;

            if (ResourceType == ResourceType.Other)
            {
                strQuery = string.Format("SELECT * FROM {0} WHERE ResourceType = \"{1}\" AND OtherResourceType = \"{2}\" AND PoolId = \"{3}\"", ClassName, (int)ResourceType, ResourceSubtype, Id);
            }
            else
            {
                strQuery = string.Format("SELECT * FROM {0} WHERE ResourceType = \"{1}\" AND ResourceSubType = \"{2}\" AND PoolId = \"{3}\"", ClassName, (int)ResourceType, ResourceSubtype, Id);
            }

            SelectQuery wqlQuery = new SelectQuery(strQuery);

            using (ManagementObjectSearcher mSearcher = new ManagementObjectSearcher(mScope, wqlQuery))
            {
                return mSearcher.Get();
            }
        }
        private static ManagementObject getManagementObject(string ClassName, ResourceType ResourceType, string ResourceSubtype, string Id, ManagementScope mScope)
        {
            string strQuery = null;

            if (ResourceType == ResourceType.Other)
            {
                strQuery = string.Format("SELECT * FROM {0} WHERE ResourceType = \"{1}\" AND OtherResourceType = \"{2}\" AND PoolId = \"{3}\"", ClassName, (int)ResourceType, ResourceSubtype, Id);
            }
            else
            {
                strQuery = string.Format("SELECT * FROM {0} WHERE ResourceType = \"{1}\" AND ResourceSubType = \"{2}\" AND PoolId = \"{3}\"", ClassName, (int)ResourceType, ResourceSubtype, Id);
            }

            SelectQuery wqlQuery = new SelectQuery(strQuery);

            using (ManagementObjectSearcher mSearcher = new ManagementObjectSearcher(mScope, wqlQuery))
            using (ManagementObjectCollection mCollection = mSearcher.Get())
            {
                if (mCollection.Count != 1)
                {
                    throw new ManagementException(string.Format("A single {0} derived instance could not be found for ResourceType \"{1}\", ResourceSubtype \"{2}\" and PoolId \"{3}\"", ClassName, ResourceType, ResourceSubtype, Id));
                }

                foreach (ManagementObject mReturn in mCollection)
                {
                    return mReturn;
                }
            }
            return null;
        }
        private static ManagementObject getManagementObject(string Name, string ClassName, ManagementScope mScope)
        {
            string strQuery = string.Format("SELECT * FROM {0} WHERE ElementName = \"{1}\"", ClassName, Name);
            SelectQuery wqlQuery = new SelectQuery(strQuery);

            using (ManagementObjectSearcher moSearcher = new ManagementObjectSearcher(mScope, wqlQuery))
            using (ManagementObjectCollection moCollection = moSearcher.Get())
            {
                if (moCollection.Count == 0)
                {
                    throw new ManagementException(string.Format("No {0} could be found with name \"{1}\"", ClassName, Name));
                }

                foreach (ManagementObject mReturn in moCollection)
                {
                    return mReturn;
                }
            }

            return null;
        }
        private static ManagementObject getManagementObject(string ClassName, ManagementScope mScope)
        {
            using (ManagementClass mClass = new ManagementClass(ClassName))
            {
                mClass.Scope = mScope;

                foreach (ManagementObject mReturn in mClass.GetInstances())
                {
                    return mReturn;
                }
            }
            return null;
        }
        private static ManagementObject getSettingData(string relatedClass, string relationshipClass, ManagementObject mObject, ManagementScope mScope)
        {
            using (ManagementObjectCollection mCollection = mObject.GetRelated(relatedClass, relationshipClass, null, null, null, null, false, null))
            {
                foreach (ManagementObject mReturn in mCollection)
                {
                    return mReturn;
                }
            }
            return null;
        }
        private static ManagementObject getSettingData(string relatedClass, ManagementScope mScope)
        {
            using (ManagementClass mClass = new ManagementClass(relatedClass))
            {
                mClass.Scope = mScope;

                foreach (ManagementObject mReturn in mClass.GetInstances())
                {
                    return mReturn;
                }
            }
            return null;
        }
        private static ManagementObject[] getVHDSettings(ManagementObject mObject)
        {
            const UInt16 SASDResourceTypeLogicalDisk = 31;

            List<ManagementObject> StorageAllocationSettingData = new List<ManagementObject>();
            using (ManagementObjectCollection mCollection = mObject.GetRelated(wmiClass.Msvm_StorageAllocationSettingData.ToString(), wmiClass.Msvm_VirtualSystemSettingDataComponent.ToString(), null, null, null, null, false, null))
            {
                foreach (ManagementObject mReturn in mCollection)
                {
                    if ((UInt16)mReturn["ResourceType"] == SASDResourceTypeLogicalDisk)
                    {
                        StorageAllocationSettingData.Add(mReturn);
                    }
                    else
                    {
                        mReturn.Dispose();
                    }
                }
            }
            if (StorageAllocationSettingData.Count == 0)
            {
                return null;
            }
            else
            {
                return StorageAllocationSettingData.ToArray();
            }
        }
    }
}