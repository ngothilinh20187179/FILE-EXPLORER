using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FILE_EXPLORER
{
    public partial class Form1 : Form
    {   
        //Khai bao bien toan cuc cho thu muc hien tai
        DirectoryInfo currentDir;

        public Form1()
        {
            InitializeComponent();
        }
        private void initialize_treeView()
        {
            //Tao 1 root node, dat ten la This PC, add root node vao treeview
            TreeNode thisPC = new TreeNode("This PC");
            thisPC.Tag = "This PC";
            // set image cho root node (This PC)
            thisPC.ImageIndex = 0;
            thisPC.SelectedImageIndex = 0;
            treeView1.Nodes.Add(thisPC); 

            // add cac disk(C,D,..) vao root node (This PC)
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeNode driveNode = new TreeNode(drive.Name);
                driveNode.Tag = drive.RootDirectory;
                // set image cho disk C, D
                driveNode.ImageIndex = 1;
                driveNode.SelectedImageIndex = 1;
                thisPC.Nodes.Add(driveNode);
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            initialize_treeView();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBox1.Text = treeView1.SelectedNode.FullPath; // sau khi chon node thi lay path hien thi vao textbox
            TreeNode selectedNode = treeView1.SelectedNode;
            try
            {
                if (selectedNode.Tag.GetType() == typeof(DirectoryInfo))
                {
                    // xoa cac node da co trong danh sach
                    selectedNode.Nodes.Clear();
                    // them cac node thu muc con
                    DirectoryInfo dir = (DirectoryInfo)selectedNode.Tag;
                    foreach (DirectoryInfo subDir in dir.GetDirectories())
                    {
                        TreeNode dirNode = new TreeNode(subDir.Name);
                        dirNode.Tag = subDir;
                        dirNode.ImageIndex = 2;
                        dirNode.SelectedImageIndex = 3;
                        selectedNode.Nodes.Add(dirNode);
                    }
                    // hien thi tren listview
                    currentDir = dir;
                    OpenDirectory();
                }
                selectedNode.Expand();
            }
            catch
            {
                MessageBox.Show("Can't open!");
                return;
            }
        }
        private void OpenDirectory()
        {
            listView1.Clear();
            // them cac cot Name, Type, Size, Data Modified vao listView1
            listView1.Columns.Add("Name", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("Size", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Type", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Date Modified", 230, HorizontalAlignment.Center);

            // hien thi theo dang chi tiet
            listView1.View = View.Details;

            foreach (DirectoryInfo subDir in currentDir.GetDirectories())
            {
                ListViewItem lv1 = listView1.Items.Add(subDir.Name);
                lv1.Tag = subDir;
                lv1.ImageIndex = 2;
                lv1.SubItems.Add("");
                lv1.SubItems.Add("Folder");
                lv1.SubItems.Add(subDir.LastWriteTime.ToString());
            }
            foreach (FileInfo file in currentDir.GetFiles())
            {
                ListViewItem lv1 = listView1.Items.Add(file.Name);
                lv1.Tag = file;
                lv1.ImageIndex = 3;
                lv1.SubItems.Add(file.Length.ToString());
                lv1.SubItems.Add("File");
                lv1.SubItems.Add(file.LastWriteTime.ToString());
            }
        }
        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0].Tag.GetType() == typeof(DirectoryInfo))
                {
                    currentDir = (DirectoryInfo)listView1.SelectedItems[0].Tag;
                    OpenDirectory();
                }
                else
                {
                    FileInfo file = (FileInfo)listView1.SelectedItems[0].Tag;
                    System.Diagnostics.Process.Start(file.FullName);
                }
            }
            catch
            {
                MessageBox.Show("Can't open!");
                return;
            }
        }
    }
}
