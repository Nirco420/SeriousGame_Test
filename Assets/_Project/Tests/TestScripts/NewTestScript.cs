using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    [Test]
    public void TwoPlusTwo()
    {
        Assert.AreEqual(4, 2 + 2);
    }
}
