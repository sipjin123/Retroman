﻿using System;
using UnityEngine;
using FLogger = Common.Logger.Logger;
using Framework;

public static class Assertion
{
    private const string DEFAULT_MESSAGE = "AssertionError";

    /**
	 * Asserts the specified expression
	 */
    public static void Assert(bool expression)
    {
        Assert(expression, DEFAULT_MESSAGE);
    }

    public static void Assert(string message)
    {
        Assert(false, message);
    }

    /**
     * Asserts the specified expression.
     */
    public static void Assert(bool expression, string format, params object[] args)
    {
        string assertErrorMessage = string.Format(format, args);
        Assert(expression, assertErrorMessage);
    }

    /**
        * Asserts the specified expression.
        */
    public static void Assert(bool expression, string assertErrorMessage)
    {
        if (!expression)
        {
            // use logger only if not in editor
#if !UNITY_EDITOR
			try {
#endif
            throw new Exception(assertErrorMessage);
#if !UNITY_EDITOR
			} catch(Exception e) {
				FLogger.GetInstance().LogError(assertErrorMessage);
				FLogger.GetInstance().LogError(D.ERROR + e.StackTrace);
				throw e;
			}
#endif
        }
    }

    /**
     * Asserts that the specified pointer is not null.
     */
    public static void AssertNotNull(object pointer, string format, params object[] args)
    {
        string assertErrorMessage = string.Format(format, args);
        AssertNotNull(pointer, assertErrorMessage);
    }

    /**
     * Asserts that the specified pointer is not null.
     */
    public static void AssertNotNull(object pointer, string name)
    {
        Assert(pointer != null, name + " should not be null");
    }

    /**
     * Asserts that the specified pointer is not null.
     */
    public static void AssertNotNull(object pointer)
    {
        Assert(pointer != null, DEFAULT_MESSAGE);
    }

    /**
	 * Asserts that the specified UnityEngine object is not null.
	 */
    public static void AssertNotNull(UnityEngine.Object pointer, string name)
    {
        if (!pointer)
        {
            Assert(false, name + " should not be null");
        }
    }

    /**
	 * Asserts that the specified UnityEngine object is not null.
	 */
    public static void AssertNotNull(UnityEngine.Object pointer)
    {
        if (!pointer)
        {
            Assert(false, DEFAULT_MESSAGE);
        }
    }

    /**
	 * Asserts that the specified string is not empty.
	 */
    public static void AssertNotEmpty(string s, string name)
    {
        Assert(!string.IsNullOrEmpty(s), name + " should not be empty");
    }

    /**
	 * Asserts that the specified string is not empty.
	 */
    public static void AssertNotEmpty(string s, string format, params object[] args)
    {
        string assertErrorMessage = string.Format(format, args);
        AssertNotEmpty(s, assertErrorMessage);
    }

    /**
	 * Asserts that the specified string is not empty.
	 */
    public static void AssertNotEmpty(string s)
    {
        Assert(!string.IsNullOrEmpty(s), DEFAULT_MESSAGE);
    }

}
