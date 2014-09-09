using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;

public class CommandInfo
{
    private string _commandText;
    private CommandType _commandType;

    public CommandType CommandType
    {
        get { return _commandType; }
        set { _commandType = value; }
    }
    public string CommandText
    {
        get { return _commandText; }
        set { _commandText = value; }
    }
    private SqlParameter[] _parameters;

    public SqlParameter[] Parameters
    {
        get { return _parameters; }
        set { _parameters = value; }
    }

    public CommandInfo(string sqlText, SqlParameter[] para, CommandType ct)
    {
        _commandText = sqlText;
        _parameters = para;
        _commandType = ct;
    }
}