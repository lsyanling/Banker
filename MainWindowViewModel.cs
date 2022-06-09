using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using static Banker.MainWindowViewModel;
using System.Security.Cryptography;
using System.Windows;

namespace Banker
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            InitializeResourceProcess();
            InitializeCommand();
            SelectedIndex = 0;
            TotalSafeSerial = "安全序列";
        }

        private void InitializeResourceProcess()
        {
            Processes = new();
            Resources = new();
            ApplyResources = new();
            AvailableResources = new();
            SafeSerials = new();

            Resources.Add(new(0, 17));
            Resources.Add(new(1, 5));
            Resources.Add(new(2, 20));

            ApplyResources.Add(new(0, 0));
            ApplyResources.Add(new(1, 0));
            ApplyResources.Add(new(2, 0));

            AvailableResources.Add(new(0));
            AvailableResources.Add(new(1));
            AvailableResources.Add(new(2));

            Processes.Add(new(0, new()
            {
                new(0, 5),
                new(1, 5),
                new(2, 9)
            }, new()
            {
                new(0, 2),
                new(1, 1),
                new(2, 2)
            }));

            Processes.Add(new(1, new()
            {
                new(0, 5),
                new(1, 3),
                new(2, 6)
            }, new()
            {
                new(0, 4),
                new(1, 0),
                new(2, 2)
            }));

            Processes.Add(new(2, new()
            {
                new(0, 4),
                new(1, 0),
                new(2, 11)
            }, new()
            {
                new(0, 4),
                new(1, 0),
                new(2, 5)
            }));

            Processes.Add(new(3, new()
            {
                new(0, 4),
                new(1, 2),
                new(2, 5)
            }, new()
            {
                new(0, 2),
                new(1, 0),
                new(2, 4)
            }));

            Processes.Add(new(4, new()
            {
                new(0, 4),
                new(1, 2),
                new(2, 4)
            }, new()
            {
                new(0, 3),
                new(1, 1),
                new(2, 4)
            }));
        }

        private void InitializeCommand()
        {
            AddResourceCommand = new()
            {
                Action = AddResource
            };

            DeleteResourceCommand = new()
            {
                Action = DeleteResource
            };

            AddProcessCommand = new()
            {
                Action = AddProcess
            };

            DeleteProcessCommand = new()
            {
                Action = DeleteProcess
            };

            RandomInputCommand = new()
            {
                Action = RandomInput
            };

            CalculateSafeSerialCommand = new()
            {
                Action = Calculate
            };

            ApplyForResourcesCommand = new()
            {
                Action = ApplyForResources
            };
        }

        private int selectedIndex;
        public int SelectedIndex
        { 
            get=> selectedIndex;
            set => SetProperty(ref selectedIndex, value); 
        }

        private string totalSafeSerial;
        public string TotalSafeSerial
        {
            get => totalSafeSerial;
            set => SetProperty(ref totalSafeSerial, value);
        }

        public ObservableCollection<Process> Processes { get; set; }
        public ObservableCollection<Resource> Resources { get; set; }
        public ObservableCollection<Resource> ApplyResources { get; set; }
        public ObservableCollection<Resource> AvailableResources { get; set; }
        public ObservableCollection<Serial> SafeSerials { get; set; }

        public MyCommand AddResourceCommand { get; set; }
        public MyCommand DeleteResourceCommand { get; set; }

        public MyCommand AddProcessCommand { get; set; }
        public MyCommand DeleteProcessCommand { get; set; }

        public MyCommand RandomInputCommand { get; set; }

        public MyCommand CalculateSafeSerialCommand { get; set; }

        public MyCommand ApplyForResourcesCommand { get; set; }

        private void AddResource(object? obj)
        {
            if (Resources.Count < 7)
            {
                Resources.Add(new(Resources.Count));
                foreach (var process in Processes)
                {
                    process.MaxResources.Add(new(process.MaxResources.Count));
                    process.NowResources.Add(new(process.NowResources.Count));
                    process.NeedResources.Add(new(process.NeedResources.Count));
                }
                ApplyResources.Add(new(ApplyResources.Count, 0));
                AvailableResources.Add(new(AvailableResources.Count, 0));
            }
        }

        private void DeleteResource(object? obj)
        {
            if (Resources.Count > 1)
            {
                Resources.RemoveAt(Resources.Count - 1);
                foreach (var process in Processes)
                {
                    process.MaxResources.RemoveAt(process.MaxResources.Count - 1);
                    process.NowResources.RemoveAt(process.NowResources.Count - 1);
                    process.NeedResources.RemoveAt(process.NeedResources.Count - 1);
                }
                ApplyResources.RemoveAt(ApplyResources.Count - 1);
                AvailableResources.RemoveAt(AvailableResources.Count - 1);
            }
        }
        private void AddProcess(object? obj)
        {
            if (Processes.Count < 11)
            {
                Processes.Add(new(Processes.Count));
                for (int i = 0; i < Resources.Count; i++)
                {
                    Processes[^1].MaxResources.Add(new(i));
                    Processes[^1].NowResources.Add(new(i));
                    Processes[^1].NeedResources.Add(new(i));
                }
            }
        }

        private void DeleteProcess(object? obj)
        {
            if (Processes.Count > 1)
            {
                Processes.RemoveAt(Processes.Count - 1);
            }
        }

        private void RandomInput(object? obj)
        {
            Random random = new();
            int randomRange = 30;
            foreach (var resource in Resources)
            {
                resource.Quantity = random.Next(1, randomRange);
            }
            for (int i = 0; i < Resources.Count; i++)
            {
                AvailableResources[i].Quantity = Resources[i].Quantity;
            }

            for (int i = 0; i < Processes.Count; i++)
            {
                for (int j = 0; j < Resources.Count; j++)
                {
                    Processes[i].MaxResources[j].Quantity = random.Next(0,
                        Resources[j].Quantity);
                    Processes[i].NowResources[j].Quantity = random.Next(0,
                        AvailableResources[j].Quantity <
                        Processes[i].MaxResources[j].Quantity ?
                        AvailableResources[j].Quantity :
                        Processes[i].MaxResources[j].Quantity);
                    AvailableResources[j].Quantity -= Processes[i].NowResources[j].Quantity;
                }
            }
        }

        private void ApplyForResources(object? obj)
        {
            try
            {
                Calculate(null);

                for (int i = 0; i < Resources.Count; i++)
                {
                    if (Processes[SelectedIndex].NeedResources[i].Quantity <
                        ApplyResources[i].Quantity)
                    {
                        throw new("进程"+ SelectedIndex.ToString()+ "  申请的  资源" + i.ToString() + "  超出声明的最大值");
                    }
                    if (AvailableResources[i].Quantity <
                        ApplyResources[i].Quantity)
                    {
                        throw new("进程" + SelectedIndex.ToString() + "  申请的  资源" + i.ToString() + "  超出当前可用资源的最大值");
                    }
                }

                for (int i = 0; i < Resources.Count; i++)
                {
                    Processes[SelectedIndex].NowResources[i].Quantity += ApplyResources[i].Quantity;
                }

                Calculate(null);

                if (SafeSerials.Count > 0)
                {

                }
                else
                {
                    for (int i = 0; i < Resources.Count; i++)
                    {
                        Processes[SelectedIndex].NowResources[i].Quantity -= ApplyResources[i].Quantity;
                    }
                    Calculate(null);
                    throw new("分配后进入不安全状态，不允许分配");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void Calculate(object? obj)
        {
            // 初始化待分配资源
            for (int i = 0; i < Resources.Count; i++)
            {
                AvailableResources[i].Quantity = Resources[i].Quantity;
            }
            foreach (var process in Processes)
            {
                for (int i = 0; i < process.NowResources.Count; i++)
                {
                    AvailableResources[i].Quantity -= process.NowResources[i].Quantity;
                }
            }

            // 初始化尚需资源
            foreach (var process in Processes)
            {
                for (int i = 0; i < process.NeedResources.Count; i++)
                {
                    process.NeedResources[i].Quantity =
                        process.MaxResources[i].Quantity - process.NowResources[i].Quantity;
                }
            }

            // 转换成数组
            List<int> available = new();
            for (int i = 0; i < AvailableResources.Count; i++)
            {
                available.Add(AvailableResources[i].Quantity);
            }
            List<List<int>> need = new();
            for (int i = 0; i < Processes.Count; i++)
            {
                need.Add(new());
                for (int j = 0; j < Processes[i].NeedResources.Count; j++)
                {
                    need[i].Add(Processes[i].NeedResources[j].Quantity);
                }
            }
            List<List<int>> now = new();
            for (int i = 0; i < Processes.Count; i++)
            {
                now.Add(new());
                for (int j = 0; j < Processes[i].NowResources.Count; j++)
                {
                    now[i].Add(Processes[i].NowResources[j].Quantity);
                }
            }

            // 求安全序列
            List<List<int>> allSafeSerial = CalculateAllSafeSerial(available, need, now);

            SafeSerials.Clear();
            // 更新输出
            foreach (var safeSerial in allSafeSerial)
            {
                string SafeSerial = "进程   " + safeSerial[0].ToString();
                for (int i = 1; i < safeSerial.Count; i++)
                {
                    SafeSerial += ",   " + safeSerial[i].ToString();
                }
                SafeSerials.Add(new(SafeSerial));
            }
            TotalSafeSerial = "安全序列    (共" + SafeSerials.Count.ToString() + "个)";
        }

        private List<List<int>> CalculateAllSafeSerial(List<int> available,
            List<List<int>> need, List<List<int>> now)
        {
            List<List<int>> allSafeSerial = new();
            List<bool> used = new();
            for (int i = 0; i < need.Count; i++)
                used.Add(false);

            CalculateSafeSerial(available, need, now, allSafeSerial, null, used);

            return allSafeSerial;
        }

        private void CalculateSafeSerial(List<int> available,
            List<List<int>> need, List<List<int>> now,
            List<List<int>> allSafeSerial, List<int>? safeSerial,
            List<bool> used)
        {
            // 顶层
            if (safeSerial is null)
            {
                for (int i = 0; i < need.Count; i++)
                {
                    bool canAccomplish = true;
                    for (int j = 0; j < need[i].Count; j++)
                    {
                        if (available[j] < need[i][j])
                        {
                            canAccomplish = false;
                            break;
                        }
                    }
                    if (canAccomplish)
                    {
                        safeSerial = new();
                        List<int> thisAvailable = new(available);
                        List<bool> thisUsed = new(used);
                        thisUsed[i] = true;
                        for (int j = 0; j < need[i].Count; j++)
                        {
                            thisAvailable[j] += now[i][j];
                        }
                        safeSerial.Add(i);
                        CalculateSafeSerial(thisAvailable, need, now, allSafeSerial, safeSerial, thisUsed);
                    }
                }
            }
            // 底层
            else if (safeSerial.Count == need.Count)
            {
                List<int> end = new(safeSerial);
                allSafeSerial.Add(end);
            }
            // 中间层
            else
            {
                for (int i = 0; i < need.Count; i++)
                {
                    if (used[i])
                        continue;
                    bool canAccomplish = true;
                    for (int j = 0; j < need[i].Count; j++)
                    {
                        if (available[j] < need[i][j])
                        {
                            canAccomplish = false;
                            break;
                        }
                    }
                    if (canAccomplish)
                    {
                        List<int> thisSerial = new(safeSerial);
                        List<int> thisAvailable = new(available);
                        List<bool> thisUsed = new(used);
                        thisUsed[i] = true;
                        for (int j = 0; j < need[i].Count; j++)
                        {
                            thisAvailable[j] += now[i][j];
                        }
                        thisSerial.Add(i);
                        CalculateSafeSerial(thisAvailable, need, now, allSafeSerial, thisSerial, thisUsed);
                    }
                }
            }
        }
    }

    public class Serial : ObservableObject
    {
        public Serial(string serial)
        {
            Str = serial;
        }

        private string str;
        public string Str
        {
            get => str;
            set => SetProperty(ref str, value);
        }
    }

    public class Process : ObservableObject
    {
        public Process(int id)
        {
            Pid = "进程" + id.ToString();
            MaxResources = new();
            NowResources = new();
            NeedResources = new();
        }

        public Process(int id, List<Resource> max) : this(id)
        {
            foreach (var resource in max)
            {
                MaxResources.Add(resource);
            }
            for (int i = 0; i < MaxResources.Count; i++)
            {
                NeedResources.Add(new(i, MaxResources[i].Quantity));
            }
        }

        public Process(int id, List<Resource> max, List<Resource> now) : this(id, max)
        {
            foreach (var resource in now)
            {
                NowResources.Add(resource);
            }
            for (int i = 0; i < NeedResources.Count; i++)
            {
                NeedResources[i].Quantity -= NowResources[i].Quantity;
            }
        }

        private string pid;
        public string Pid
        {
            get => pid;
            set => SetProperty(ref pid, value);
        }

        public ObservableCollection<Resource> NowResources { get; set; }

        public ObservableCollection<Resource> MaxResources { get; set; }

        public ObservableCollection<Resource> NeedResources { get; set; }
    }

    public class Resource : ObservableObject
    {
        public Resource(int id)
        {
            Rid = "资源" + id.ToString();
            Quantity = 0;
        }

        public Resource(int id, int quantity)
        {
            Rid = "资源" + id.ToString();
            Quantity = quantity;
        }

        private string rid;
        public string Rid
        {
            get => rid;
            set => SetProperty(ref rid, value);
        }

        private int quantity;
        public int Quantity
        {
            get => quantity;
            set => SetProperty(ref quantity, value);
        }
    }

    public class MyCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Action?.Invoke(parameter);
        }

        public Action<object?>? Action { get; set; }
    }
}
