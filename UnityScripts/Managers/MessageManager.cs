using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TTG.NavalWar.NWComms.Entities;

public class MessageManager : MonoBehaviour
{

    private List<Message> Messages = new List<Message>();
    private List<Message> NonArchiveMessages = new List<Message>();
    private List<Message> _BattleMessages = new List<Message>();
    private List<Message> _DetectionMessages = new List<Message>();
    private List<Message> _GameMessages = new List<Message>();
    private List<Message> _ArchiveMessages = new List<Message>();


    private bool _Changed = false;


    private string _LastStr = "Test";
    #region Script variables


    public int NumberOfMessagesInList = 5;
    public InteractionWindow GUIWindow;


    #endregion

    public bool Changed
    {
        get
        {
            return _Changed;
        }
    }

    public void AddMessage(string message, GameManager.MessageTypes messageType, PositionInfo position)
    {
        Message m = new Message(message, messageType);
        m.Position = position;
        Messages.Add(m);

        if (Messages.Count > NumberOfMessagesInList && NumberOfMessagesInList != -1)
        {
            RemoveOldestMessage();
        }

        _Changed = true;
        SortMessagesByDate(true);
        //ShowPopup(m);
        GUIWindow.StackMessages.Add(new StackMessage(m, GameManager.Instance.GUIManager.PopupWindowTimer));
        
    }

    private void RemoveOldestMessage()
    {
        DateTime newest = DateTime.Now;
        Message messageToDelete = new Message();

        foreach (Message m in Messages)
        {
            if (m.Date < messageToDelete.Date)
            {
                messageToDelete = m;
            }
        }

        if (messageToDelete != null)
        {
            Messages.Remove(messageToDelete);
        }
    }

    public void RemoveMessage(Message m)
    {
        Messages.Remove(m);
        SortMessagesByDate(true);
    }

    public void ArchiveMessage(Message m)
    {
        m.MessageType = GameManager.MessageTypes.Archive;
        SortMessagesByDate(true);
    }

    public override string ToString()
    {
        //~ Debug.Log("Getting message");
        if (!_Changed)
        {
            return _LastStr;
        }
        else
        {
            string ret = "";
            for (int i = Messages.Count - 1; i > 0; --i)
            {
                Debug.Log(Messages[i].Header);
                ret += Messages[i].Header + "\n";
            }

            _Changed = false;
            _LastStr = ret;
            //~ Debug.Log(string.Format("Returning string {0}", ret));
            return ret;
        }

    }

    public void ShowPopup(Message messageToDisplay)
    {
        GameObject go = new GameObject("Alert");
        //PopupWindow p = go.AddComponent(typeof(PopupWindow)) as PopupWindow;
        //p.MessageToDisplay = messageToDisplay;

        PopupWindow flash = go.AddComponent<PopupWindow>();
        flash.MessageToDisplay = messageToDisplay;
    }

    public List<Message> GetMessagesByType(GameManager.MessageTypes type)
    {
        List<Message> messages = Messages.FindAll(delegate(Message m) { return m.MessageType == type; });
        messages.Sort(delegate(Message m1, Message m2) { return m2.Date.CompareTo(m1.Date); });
        return messages;
    }

    public List<Message> AllMessages
    {
        get
        {
            return NonArchiveMessages;
        }
    }
    public List<Message> BattleMessages
    {
        get
        {
            return _BattleMessages;
        }
    }

    public List<Message> ArchiveMessages
    {
        get
        {
            return _ArchiveMessages;
        }
    }

    public List<Message> GameMessages
    {
        get
        {
            return _GameMessages;
        }
    }

    public List<Message> DetectionMessages
    {
        get
        {
            return _DetectionMessages;
        }
    }

    

    private void SortMessagesByDate(bool ascending)
    {
        Messages.Sort(delegate(Message m1, Message m2) { return ascending == true ? m2.Date.CompareTo(m1.Date) : m1.Date.CompareTo(m2.Date); });
        NonArchiveMessages = Messages.FindAll(delegate(Message m1) { return m1.MessageType != GameManager.MessageTypes.Archive; });
        _BattleMessages = GetMessagesByType(GameManager.MessageTypes.Battle);
        _ArchiveMessages = GetMessagesByType(GameManager.MessageTypes.Archive);
        _GameMessages = GetMessagesByType(GameManager.MessageTypes.Game);
        _DetectionMessages = GetMessagesByType(GameManager.MessageTypes.Detection);
    }

    public int GetMessageTypeCount(GameManager.MessageTypes messageType, bool onlyUnread)
    {
        return (Messages.FindAll(delegate(Message m)
                                                    {
                                                        if (onlyUnread)
                                                        {
                                                            return m.MessageType == messageType && m.IsRead == false;
                                                        }
                                                        else
                                                        {
                                                            return m.MessageType == messageType;
                                                        }
                                                    })).Count;
    }

}

public class Message
{
    private string _Header;
    private string _Body;
    private DateTime _Date;
    private GameManager.MessageTypes _MessageType;
    private bool _IsRead = false;
    private bool _HasBody = false;
    private PositionInfo _Position;

    public Message()
    {
        _Header = "";
        _Date = DateTime.Now;
    }

    public Message(string message)
    {
        _Header = message;
        _Date = DateTime.Now;


    }



    public Message(string message, GameManager.MessageTypes messageType)
    {

        if (message.Length > GameManager.Instance.MaxMessageLength)
        {
            _Header = message.Substring(0, GameManager.Instance.MaxMessageLength) + "...";
            _Body = message;
            _HasBody = true;
        }
        else
        {
            _Header = message;
            _Body = message;
        }

        _Date = DateTime.Now;
        _MessageType = messageType;
    }

    public PositionInfo Position
    {
        get
        {
            return _Position;
        }
        set
        {
            _Position = value;
        }
    }


    public string Header
    {
        get
        {
            return _Header;
        }
        set
        {
            _Header = value;
        }
    }

    public string Body
    {
        get
        {
            return _Body;
        }
        set
        {
            _Body = value;
        }
    }

    public DateTime Date
    {
        get
        {
            return _Date;
        }
        set
        {
            _Date = value;
        }
    }

    public GameManager.MessageTypes MessageType
    {
        get
        {
            return _MessageType;
        }

        set
        {
            _MessageType = value;
        }
    }

    public bool IsRead
    {
        get
        {
            return _IsRead;
        }
        set
        {
            _IsRead = value;
        }
    }

    public bool HasBody
    {
        get
        {
            return _HasBody;
        }
        set
        {
            _HasBody = value;
        }
    }

    public GUIStyle Style
    {
        get
        {
            return GameManager.Instance.GUIManager.GetMessageStyleByType(MessageType, IsRead, -1);
        }
    }

    public GUIStyle GetStyle(int mode)
    {
        return GameManager.Instance.GUIManager.GetMessageStyleByType(MessageType, IsRead, mode);
    }

    public GUIStyle GetArchiveDeleteButtonStyle(bool deleteButton, int mode)
    {
        return GameManager.Instance.GUIManager.GetArchiveStyleByType(deleteButton, mode);
    }

    public override string ToString()
    {
        return Header;
    }
}
