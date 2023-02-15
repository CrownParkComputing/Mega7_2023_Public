using CG.Web.MegaApiClient;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mega7_2023
{
    public class ViewModel : NotificationObject
    {
        MegaApiClient client = new();
        Uri folderLink = new("https://mega.nz/folder/gdozjZxL#uI5SheetsAd-NYKMeRjf2A");
        IEnumerable<CG.Web.MegaApiClient.INode> nodes;


        public ViewModel()
        {
            {              
                
                this.Nodes = GetNodesCollection();
                rowDataCommand = new RelayCommand(ChangeCanExecute);
            }
        }

        private ObservableCollection<Node> _Nodes;
        /// <summary>
        /// Gets or sets the employee info.
        /// </summary>
        /// <value>The employee info.</value>
        public ObservableCollection<Node> Nodes
        {
            get
            {
                return _Nodes;
            }
            set
            {
                _Nodes = value;
            }
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        private ICommand rowDataCommand { get; set; }
        public ICommand RowDataCommand
        {
            get
            {
                return rowDataCommand;
            }
            set
            {
                rowDataCommand = value;
            }
        }

        public void ChangeCanExecute(object obj)
        {
            var rowdataContent = (obj as Node);

            if (rowdataContent.Type == NodeType.File)
            {
                DownloadFile(rowdataContent.NodeId);
                MessageBox.Show("Finished Downloading File");
            }
            else
            {
                DownloadFolder(rowdataContent);
                MessageBox.Show("Finished Downloading Folder");
            }
        }

        List<string> GetParents(INode node, IEnumerable<INode> nodes)
        {
            List<string> parents = new();

            string SaveDirectory = "E:";

            while (node.ParentId != null)
            {
                INode parentNode = nodes.Single(x => x.Id == node.ParentId);
                parents.Insert(0, parentNode.Name);
                node = parentNode;
            }
            parents.Insert(0,SaveDirectory);
            return parents;
        }

        public ObservableCollection<Node> GetNodesCollection()
        {
            ObservableCollection<Node> _nodesCollection = new();
            {
                client.LoginAnonymous();
                //client.Login("username@domain.com", "passw0rd");
            }

            nodes = client.GetNodesFromLink(folderLink);

            CG.Web.MegaApiClient.INode root;
            root = nodes.Single(n => n.Type == NodeType.Root);

            Node rootNode = new Node() { NodeId = root.Id, Type = root.Type, Name = root.Name, Size = root.Size, CreationDate = (DateTime)root.CreationDate };

            GetNodesRecursive(rootNode, nodes, root);
            _nodesCollection.Add(rootNode);

            return _nodesCollection;
        }



        public void DownloadFile(string nodeId)
        {
            CG.Web.MegaApiClient.INode node = nodes.Single(x => x.Id == nodeId);

            List<string> parents = GetParents(node, nodes);


            string tempFolder = string.Join('\\', parents);

            Directory.CreateDirectory(tempFolder);

            string fileToSave = Path.Combine(tempFolder, node.Name);

            if (File.Exists(fileToSave))
            {
                File.Delete(fileToSave);
            }

            if (!client.IsLoggedIn)
            { client.LoginAnonymous(); }

            client.DownloadFile(node, fileToSave, null);
            if ((DateTime?)node.CreationDate != null)
            {
                File.SetCreationTime(fileToSave, (DateTime)node.CreationDate);
                File.SetLastWriteTime(fileToSave, (DateTime)node.CreationDate);
            }
        }

        public void DownloadFolder(Node folderNode)   // need to upgrade this to task / async download 
        {
            if (!client.IsLoggedIn)
            {
                client.LoginAnonymous();
                //client.Login("username@domain.com", "passw0rd");
            }

            foreach (Node temp in folderNode.Items)
            {
                if (temp.Type == NodeType.File)
                {

                    CG.Web.MegaApiClient.INode node = nodes.Single(x => x.Id == temp.NodeId);

                    List<string> parents = GetParents(node, nodes);
                    string tempFolder = string.Join('\\', parents);

                    Directory.CreateDirectory(tempFolder);

                    string fileToSave = Path.Combine(tempFolder, node.Name);

                    if (File.Exists(fileToSave))
                    {
                        File.Delete(fileToSave);
                    }
                    client.DownloadFile(node, fileToSave, null);

                    if ((DateTime?)node.CreationDate != null)
                    {
                        File.SetCreationTime(fileToSave, (DateTime)node.CreationDate);
                        File.SetLastWriteTime(fileToSave, (DateTime)node.CreationDate);
                    }
                }

                if (temp.Type == NodeType.Directory)
                {
                    DownloadFolder(temp);
                }

            }
        }



        void GetNodesRecursive(Node rootNode , IEnumerable<CG.Web.MegaApiClient.INode> nodes, CG.Web.MegaApiClient.INode parent, int level = 0)
        {
            
            IEnumerable<CG.Web.MegaApiClient.INode> children = nodes.Where(x => x.ParentId == parent.Id);

            foreach (CG.Web.MegaApiClient.INode child in children)
            {
                Node _nextNode = new() { NodeId = child.Id, Type = child.Type, Name = child.Name, Size = child.Size, ParentId = child.ParentId, CreationDate = (DateTime)child.CreationDate};
                
                if (child.Type == NodeType.Directory)
                {
                    rootNode.Items??= new ObservableCollection<Node>();  
                    rootNode.Items.Add(_nextNode);
                    GetNodesRecursive(_nextNode, nodes, child, level + 1);
                       
                }
                else
                {
                    rootNode.Items??= new ObservableCollection<Node>();  
                    rootNode.Items.Add(new Node() { NodeId = child.Id, Type = child.Type, Name = child.Name, Size = child.Size, ParentId = child.ParentId, CreationDate = (DateTime)child.CreationDate });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }

}

