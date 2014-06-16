using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descrizione di riepilogo per Class1
/// </summary>
public class A
{
    public string cntr{get;set;}

    public A a1 { get; set; }

	public A()
	{
        A a = new A();
        a.cntr = "Prabhu";
	}
}