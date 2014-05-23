namespace HyperV
{
    using System;
    using System.Management;
    using System.Security;
    public class Credentials
    {
        public static string username;
        public static SecureString password;
        public static string domain;
        public static ConnectionOptions conOpts = new ConnectionOptions();
    }
    public class HyperV
    {
        public static void NewVM(string serverName, string vmName)
        {
            ManagementPath classPath = new ManagementPath()
            {
                Server = serverName,
                NamespacePath = @"\root\virtualization\v2",
                ClassName = "Msvm_VirtualSystemSettingData"
            };

            using (ManagementClass virtualSystemSettingClass = new ManagementClass(classPath))
            {
                Credentials.conOpts.Username = Credentials.username;
                Credentials.conOpts.SecurePassword = Credentials.password;
                Credentials.conOpts.Authority = Credentials.domain;
                Credentials.conOpts.Impersonation = ImpersonationLevel.Impersonate;

                ManagementScope scope = new ManagementScope(@"\\" + serverName + @"\root\virtualization\v2", Credentials.conOpts);

                virtualSystemSettingClass.Scope = scope;

                using (ManagementObject virtualSystemSetting = virtualSystemSettingClass.CreateInstance())
                {
                    virtualSystemSetting["ElementName"] = vmName;
                    virtualSystemSetting["VirtualsystemSubtype"] = "Microsoft:Hyper-V:Subtype:2";

                    string embeddedInstance = virtualSystemSetting.GetText(TextFormat.WmiDtd20);

                    using (ManagementObject service = Functions.getVMManagementService(scope))
                    using (ManagementBaseObject inParams = service.GetMethodParameters("DefineSystem"))
                    {
                        inParams["SystemSettings"] = embeddedInstance;

                        using (ManagementBaseObject outParams = service.InvokeMethod("DefineSystem", inParams, null))
                        {
                            Console.WriteLine("ret={0}", outParams["ReturnValue"]);
                        }
                    }
                }
            }
        }
        public static void newVhd(string serverName, string diskType, string diskFormat, string path, string parentPath, int maxInternalSize, int blockSize, int logicalSectorSize, int physicalSectorSize)
        {
            ManagementPath classPath = new ManagementPath()
            {
                Server = serverName,
                NamespacePath = @"\root\virtualization\v2",
                ClassName = "Msvm_VirtualHardDiskSettingData"
            };

            using (ManagementClass settingsClass = new ManagementClass(classPath))
            {
                Credentials.conOpts.Username = Credentials.username;
                Credentials.conOpts.SecurePassword = Credentials.password;
                Credentials.conOpts.Authority = Credentials.domain;
                Credentials.conOpts.Impersonation = ImpersonationLevel.Impersonate;

                ManagementScope scope = new ManagementScope(@"\\" + serverName + @"\root\virtualization\v2", Credentials.conOpts);

                settingsClass.Scope = scope;
                using (ManagementObject settingsInstance = settingsClass.CreateInstance())
                {
                    settingsInstance["Type"] = diskType;
                    settingsInstance["Format"] = diskFormat;
                    settingsInstance["Path"] = path;
                    settingsInstance["ParentPath"] = parentPath;
                    settingsInstance["MaxInternalSize"] = maxInternalSize;
                    settingsInstance["BlockSize"] = blockSize;
                    settingsInstance["LogicalSectorSize"] = logicalSectorSize;
                    settingsInstance["PhysicalSectorSize"] = physicalSectorSize;

                    string settingsInstanceString = settingsInstance.GetText(TextFormat.WmiDtd20);

                    // return settingsInstanceString;
                }
            }

        }
        public static void SetVmMemory(string serverName, string vmName, Int64 virtualQuantity)
        {
            Credentials.conOpts.Username = Credentials.username;
            Credentials.conOpts.SecurePassword = Credentials.password;
            Credentials.conOpts.Authority = Credentials.domain;
            Credentials.conOpts.Impersonation = ImpersonationLevel.Impersonate;

            ManagementScope scope = new ManagementScope(@"\\" + serverName + @"\root\virtualization\v2", Credentials.conOpts);
            ManagementObject virtualMachine = Functions.getVM(vmName, scope);
            ManagementObject memorySetting = null;

            ManagementObject virtualMachineSettings = Functions.getVMSettingData(virtualMachine, scope);
            foreach (ManagementObject memorySettingData in virtualMachineSettings.GetRelated("Msvm_MemorySettingData"))
            {
                memorySetting = memorySettingData;
                break;
            }

            string[] resourceData = new string[1];
            memorySetting["VirtualQuantity"] = virtualQuantity;
            resourceData[0] = memorySetting.GetText(TextFormat.WmiDtd20);

            using (ManagementObject virtualSystem = Functions.getVMManagementService(scope))
            using (ManagementBaseObject inParams = virtualSystem.GetMethodParameters("ModifyResourceSettings"))
            {
                inParams["ResourceSettings"] = resourceData;

                using (ManagementBaseObject outParams = virtualSystem.InvokeMethod("ModifyResourceSettings", inParams, null))
                {
                    Console.WriteLine("ret={0}", outParams["ReturnValue"]);
                }
            }
        }
        public static void SetVmProcessor(string serverName, string vmName, Int64 virtualQuantity)
        {
            Credentials.conOpts.Username = Credentials.username;
            Credentials.conOpts.SecurePassword = Credentials.password;
            Credentials.conOpts.Authority = Credentials.domain;
            Credentials.conOpts.Impersonation = ImpersonationLevel.Impersonate;

            ManagementScope scope = new ManagementScope(@"\\" + serverName + @"\root\virtualization\v2", Credentials.conOpts);
            ManagementObject virtualMachine = Functions.getVM(vmName, scope);
            ManagementObject processorSetting = null;

            ManagementObject virtualMachineSettings = Functions.getVMSettingData(virtualMachine, scope);
            foreach (ManagementObject processorSettingData in virtualMachineSettings.GetRelated("Msvm_ProcessorSettingData"))
            {
                processorSetting = processorSettingData;
                break;
            }

            string[] resourceData = new string[1];
            processorSetting["VirtualQuantity"] = virtualQuantity;
            resourceData[0] = processorSetting.GetText(TextFormat.WmiDtd20);

            using (ManagementObject virtualSystem = Functions.getVMManagementService(scope))
            using (ManagementBaseObject inParams = virtualSystem.GetMethodParameters("ModifyResourceSettings"))
            {
                inParams["ResourceSettings"] = resourceData;

                using (ManagementBaseObject outParams = virtualSystem.InvokeMethod("ModifyResourceSettings", inParams, null))
                {
                    Console.WriteLine("ret={0}", outParams["ReturnValue"]);
                }
            }
        }
        //public static void NewVhd(string serverName, string vmName, Int64 vhdSize, VirtualHardDiskFormat vhdFormat = VirtualHardDiskFormat.Vhdx, VirtualHardDiskType vhdType = VirtualHardDiskType.DynamicallyExpanding, Int32 vhdBlockSize = 33554432, Int32 vhdLogicalSectorSize = 512, Int32 vhdPhysicalSectorSize = 4096)
        //{
        //    Credentials.conOpts.Username = Credentials.username;
        //    Credentials.conOpts.SecurePassword = Credentials.password;
        //    Credentials.conOpts.Authority = Credentials.domain;
        //    Credentials.conOpts.Impersonation = ImpersonationLevel.Impersonate;

        //    ManagementScope scope = new ManagementScope(@"\\" + serverName + @"\root\virtualization\v2", Credentials.conOpts);
        //    ManagementObject virtualMachine = Functions.getVM(vmName, scope);
        //    ManagementObject virtualMachineSettings = Functions.getVMSettingData(virtualMachine, scope);
        //    ManagementObject virtualServiceSettingsData = Functions.getVMManagementServiceSetting(scope);

        //    //
        //    // This path should come from the hyper-v host (serverName)
        //    //
        //    string vhdPath = virtualServiceSettingsData["DefaultVirtualHardDiskPath"].ToString();
        //    //
        //    // This should probably be named after the GUID of the VM, and an index to account for multiple disks
        //    //
        //    string vhdName = virtualMachineSettings["VirtualSystemIdentifier"].ToString();
        //    vhdPath += "\\" + vhdName + ".vhdx";

        //    VirtualHardDiskSettingData settingData = new VirtualHardDiskSettingData(
        //        vhdType,
        //        vhdFormat,
        //        vhdPath,
        //        null,
        //        vhdSize,
        //        vhdBlockSize,
        //        vhdLogicalSectorSize,
        //        vhdPhysicalSectorSize);

        //    using (ManagementObject imageManagementService = Functions.getImageManagementService(scope))
        //    {
        //        using (ManagementBaseObject inParams = imageManagementService.GetMethodParameters("CreateVirtualHardDisk"))
        //        {
        //            inParams["VirtualDiskSettingData"] = settingData.GetVirtualHardDiskSettingDataEmbeddedInstance(serverName, imageManagementService.Path.Path, scope);

        //            using (ManagementBaseObject outParams = imageManagementService.InvokeMethod("CreateVirtualHardDisk", inParams, null))
        //            {

        //                Console.WriteLine(WmiUtilities.ValidateOutput(outParams, scope));
        //            }
        //        }
        //    }
        //}
        //public static void AttachVhd(string serverName, string vmName, string vhdPath)
        //{
        //    Credentials.conOpts.Username = Credentials.username;
        //    Credentials.conOpts.SecurePassword = Credentials.password;
        //    Credentials.conOpts.Authority = Credentials.domain;
        //    Credentials.conOpts.Impersonation = ImpersonationLevel.Impersonate;

        //    ManagementScope scope = new ManagementScope(@"\\" + serverName + @"\root\virtualization\v2", Credentials.conOpts);
        //    ManagementObject virtualMachine = WmiUtilities.GetVirtualMachine(vmName, scope);
        //    ManagementObject virtualMachineSettings = WmiUtilities.GetVirtualMachineSettings(virtualMachine);
        //}
        //public static void GetVhostPaths(string serverName)
        //{
        //    Credentials.conOpts.Username = Credentials.username;
        //    Credentials.conOpts.SecurePassword = Credentials.password;
        //    Credentials.conOpts.Authority = Credentials.domain;
        //    Credentials.conOpts.Impersonation = ImpersonationLevel.Impersonate;

        //    ManagementScope scope = new ManagementScope(@"\\" + serverName + @"\root\virtualization\v2", Credentials.conOpts);
        //    ManagementObject virtualServiceSettingsData = WmiUtilities.GetVirtualMachineManagementServiceSettings(scope);
        //    Console.WriteLine(virtualServiceSettingsData["DefaultVirtualHardDiskPath"]);
        //}
    }
}