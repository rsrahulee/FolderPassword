using System.Windows;
using System.IO;
using System.Xml;
using System;
using System.Security.AccessControl;

namespace PasswordProtection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String hexID = ".{2559a1f2-21d7-11d4-bdaf-00c04f60b9f0}";//class ID from the Windows registry, it saves the file with ".bat" extension,CLSID – class identifier; (Stored in the registry at HKEY_CLASSES_ROOT\CLSID)
        private String directory = "C:\\RR";
        private String dirPin = "\\pin.xml";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLock(object sender, RoutedEventArgs e)
        {
            if (!(boxPassword.Password.Equals("")))
            {
                DirectoryInfo d = new DirectoryInfo(directory);
                String selectedpath = d.Parent.FullName + d.Name;

                XmlDocument xmldoc = new XmlDocument();
                XmlElement xmlelem;
                XmlNode xmlnode;
                XmlText xmltext;
                xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                xmldoc.AppendChild(xmlnode);
                xmlelem = xmldoc.CreateElement("", "ROOT", "");
                xmltext = xmldoc.CreateTextNode(boxPassword.Password);
                xmlelem.AppendChild(xmltext);
                xmldoc.AppendChild(xmlelem);
                xmldoc.Save(selectedpath + dirPin);

                if (!d.Root.Equals(d.Parent.FullName))
                    d.MoveTo(d.Parent.FullName + "\\" + d.Name + hexID);

                doNotModifyFolder();//do not allow to modify
            }
        }

        private void btnUnlock(object sender, RoutedEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(directory + hexID);
            String selectedpath = d.Parent.FullName + d.Name;

            bool s = checkpassword(selectedpath);
            if (s)
            {
                modifyFolder();//allow to modify

                File.Delete(selectedpath + dirPin);
                d.MoveTo(selectedpath.Substring(0, selectedpath.LastIndexOf(".")));
            }
            else
            {
                MessageBox.Show("Incorrect Password");
            }
        }

        private bool checkpassword(String selectedpath)
        {
            if (!(boxPassword.Password.ToString().Equals("")))
            {
                XmlTextReader read = new XmlTextReader(selectedpath + dirPin);

                if (read.ReadState == ReadState.Error)
                    return false;
                else
                {
                    try
                    {
                        while (read.Read())
                            if (read.NodeType == XmlNodeType.Text)
                            {
                                if (boxPassword.Password.Equals(read.Value))
                                {
                                    read.Close();
                                    return true;
                                }
                                else
                                {
                                    read.Close();
                                    return false;
                                }
                            }
                    }
                    catch
                    {
                        return false;
                    }
                }
                read.Close();
            }
            return false;
        }

        private void btnAdd(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(directory + hexID + "\\Rahul"))
            {
                DirectoryInfo di = Directory.CreateDirectory(directory + hexID + "\\Rahul");
            }

            //modifyFolder();

            //doNotModifyFolder();
        }

        private void doNotModifyFolder()//do not allow to modify
        {
            try
            {
                string adminUserName = Environment.UserName;
                DirectorySecurity dirService = Directory.GetAccessControl(directory + hexID);
                FileSystemAccessRule fsa = new FileSystemAccessRule(adminUserName, FileSystemRights.Modify , AccessControlType.Deny);

                dirService.AddAccessRule(fsa);//add
                Directory.SetAccessControl(directory + hexID, dirService);
            }
            catch (Exception)
            {

            }
        }

        private void modifyFolder()//allow to modify
        {
            try
            {
                string adminUserName = Environment.UserName;
                DirectorySecurity dirService = Directory.GetAccessControl(directory + hexID);
                FileSystemAccessRule fsa = new FileSystemAccessRule(adminUserName, FileSystemRights.Modify , AccessControlType.Deny);

                dirService.RemoveAccessRule(fsa);//remove
                Directory.SetAccessControl(directory + hexID, dirService);
            }
            catch (Exception)
            {

            }
        }

        private void btnChangePassword(object sender, RoutedEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(directory + hexID);
            String selectedpath = d.Parent.FullName + d.Name;

             bool s = checkpassword(selectedpath);
             if (s)
             {
                 XmlDocument xmldoc = new XmlDocument();
                 xmldoc.Load(selectedpath + dirPin);
                 XmlNode pinNode = xmldoc.SelectSingleNode("/ROOT");
                 if (pinNode != null)
                 {
                     pinNode.InnerText = "radha";
                 }
                 xmldoc.Save(selectedpath + dirPin);
             }
             else
             {
                 MessageBox.Show("Wrong password");
             }
        }
    }
}
