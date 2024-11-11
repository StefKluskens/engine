using GingerEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace GingerEditor.GameProject
{
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}"; }

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }

    //Remembers location of created projects + displays them in the project list
    class OpenProject
    {
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\GingerEditor\";
        private static readonly string _projectDataPath;

        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        static OpenProject()
        {
            try
            {
                if (!Directory.Exists(_applicationDataPath))
                {
                    Directory.CreateDirectory(_applicationDataPath);
                }

                _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);

                ReadProjectData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //TODO: Log error
            }
        }

        public static Project Open(ProjectData projectData)
        {
            ReadProjectData();

            var project = _projects.FirstOrDefault(p => p.FullPath == projectData.FullPath);
            if (project != null)
            {
                project.Date = DateTime.Now;
            }
            else
            {
                //Project not in the list yet, open called after creating a new project
                //Project needs to be added to list of projects
                project = projectData;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }

            WriteProjectData();

            return Project.LoadProject(project.FullPath);
        }

        private static void ReadProjectData()
        {
            if (File.Exists(_projectDataPath))
            {
                var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(p => p.Date);
                _projects.Clear();

                foreach (var project in projects)
                {
                    if (File.Exists(project.FullPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.ginger\Icon.jpg");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.ginger\Screenshot.jpg");
                        _projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            var projects = _projects.OrderBy(p => p.Date).ToList();
            Serializer.ToFile(new ProjectDataList { Projects = projects }, _projectDataPath);
        }
    }
}
