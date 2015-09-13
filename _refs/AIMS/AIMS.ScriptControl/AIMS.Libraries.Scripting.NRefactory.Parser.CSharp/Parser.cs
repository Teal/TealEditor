using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.CSharp
{
	internal sealed class Parser : AbstractParser
	{
		private const int maxT = 125;

		private const bool T = true;

		private const bool x = false;

		private static bool[,] set = new bool[,]
		{
			{
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				true,
				false,
				false
			},
			{
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				true,
				false,
				false
			},
			{
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				true,
				true,
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				true,
				false,
				false,
				false,
				true,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			},
			{
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false
			}
		};

		private Lexer lexer;

		private StringBuilder qualidentBuilder = new StringBuilder();

		private Token t
		{
			[DebuggerStepThrough]
			get
			{
				return this.lexer.Token;
			}
		}

		private Token la
		{
			[DebuggerStepThrough]
			get
			{
				return this.lexer.LookAhead;
			}
		}

		private void CS()
		{
			this.lexer.NextToken();
			this.compilationUnit = new CompilationUnit();
			while (this.la.kind == 120)
			{
				this.UsingDirective();
			}
			while (this.IsGlobalAttrTarget())
			{
				this.GlobalAttributeSection();
			}
			while (this.StartOf(1))
			{
				this.NamespaceMemberDecl();
			}
			base.Expect(0);
		}

		private void UsingDirective()
		{
			string qualident = null;
			TypeReference aliasedType = null;
			base.Expect(120);
			Location startPos = this.t.Location;
			this.Qualident(out qualident);
			if (this.la.kind == 3)
			{
				this.lexer.NextToken();
				this.NonArrayType(out aliasedType);
			}
			base.Expect(11);
			if (qualident != null && qualident.Length > 0)
			{
				INode node;
				if (aliasedType != null)
				{
					node = new UsingDeclaration(qualident, aliasedType);
				}
				else
				{
					node = new UsingDeclaration(qualident);
				}
				node.StartLocation = startPos;
				node.EndLocation = this.t.EndLocation;
				this.compilationUnit.AddChild(node);
			}
		}

		private void GlobalAttributeSection()
		{
			base.Expect(18);
			Location startPos = this.t.Location;
			base.Expect(1);
			if (this.t.val != "assembly")
			{
				this.Error("global attribute target specifier (\"assembly\") expected");
			}
			string attributeTarget = this.t.val;
			List<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute> attributes = new List<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute>();
			base.Expect(9);
			AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute;
			this.Attribute(out attribute);
			attributes.Add(attribute);
			while (this.NotFinalComma())
			{
				base.Expect(14);
				this.Attribute(out attribute);
				attributes.Add(attribute);
			}
			if (this.la.kind == 14)
			{
				this.lexer.NextToken();
			}
			base.Expect(19);
			AttributeSection section = new AttributeSection(attributeTarget, attributes);
			section.StartLocation = startPos;
			section.EndLocation = this.t.EndLocation;
			this.compilationUnit.AddChild(section);
		}

		private void NamespaceMemberDecl()
		{
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList i = new ModifierList();
			if (this.la.kind == 87)
			{
				this.lexer.NextToken();
				Location startPos = this.t.Location;
				string qualident;
				this.Qualident(out qualident);
				INode node = new NamespaceDeclaration(qualident);
				node.StartLocation = startPos;
				this.compilationUnit.AddChild(node);
				this.compilationUnit.BlockStart(node);
				base.Expect(16);
				while (this.la.kind == 120)
				{
					this.UsingDirective();
				}
				while (this.StartOf(1))
				{
					this.NamespaceMemberDecl();
				}
				base.Expect(17);
				if (this.la.kind == 11)
				{
					this.lexer.NextToken();
				}
				node.EndLocation = this.t.EndLocation;
				this.compilationUnit.BlockEnd();
			}
			else if (this.StartOf(2))
			{
				while (this.la.kind == 18)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				while (this.StartOf(3))
				{
					this.TypeModifier(i);
				}
				this.TypeDecl(i, attributes);
			}
			else
			{
				base.SynErr(126);
			}
		}

		private void Qualident(out string qualident)
		{
			base.Expect(1);
			this.qualidentBuilder.Length = 0;
			this.qualidentBuilder.Append(this.t.val);
			while (this.DotAndIdent())
			{
				base.Expect(15);
				base.Expect(1);
				this.qualidentBuilder.Append('.');
				this.qualidentBuilder.Append(this.t.val);
			}
			qualident = this.qualidentBuilder.ToString();
		}

		private void NonArrayType(out TypeReference type)
		{
			int pointer = 0;
			type = null;
			if (this.la.kind == 1 || this.la.kind == 90 || this.la.kind == 107)
			{
				this.ClassType(out type, false);
			}
			else if (this.StartOf(4))
			{
				string name;
				this.SimpleType(out name);
				type = new TypeReference(name);
			}
			else if (this.la.kind == 122)
			{
				this.lexer.NextToken();
				base.Expect(6);
				pointer = 1;
				type = new TypeReference("void");
			}
			else
			{
				base.SynErr(127);
			}
			if (this.la.kind == 12)
			{
				this.NullableQuestionMark(ref type);
			}
			while (this.IsPointer())
			{
				base.Expect(6);
				pointer++;
			}
			if (type != null)
			{
				type.PointerNestingLevel = pointer;
			}
		}

		private void Attribute(out AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute)
		{
			string alias = null;
			if (this.la.kind == 1 && this.Peek(1).kind == 10)
			{
				this.lexer.NextToken();
				alias = this.t.val;
				base.Expect(10);
			}
			string qualident;
			this.Qualident(out qualident);
			List<Expression> positional = new List<Expression>();
			List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
			string name = (alias != null && alias != "global") ? (alias + "." + qualident) : qualident;
			if (this.la.kind == 20)
			{
				this.AttributeArguments(positional, named);
			}
			attribute = new AIMS.Libraries.Scripting.NRefactory.Ast.Attribute(name, positional, named);
		}

		private void AttributeArguments(List<Expression> positional, List<NamedArgumentExpression> named)
		{
			bool nameFound = false;
			string name = "";
			base.Expect(20);
			if (this.StartOf(5))
			{
				if (this.IsAssignment())
				{
					nameFound = true;
					this.lexer.NextToken();
					name = this.t.val;
					base.Expect(3);
				}
				Expression expr;
				this.Expr(out expr);
				if (expr != null)
				{
					if (name == "")
					{
						positional.Add(expr);
					}
					else
					{
						named.Add(new NamedArgumentExpression(name, expr));
						name = "";
					}
				}
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					if (this.IsAssignment())
					{
						nameFound = true;
						base.Expect(1);
						name = this.t.val;
						base.Expect(3);
					}
					else if (this.StartOf(5))
					{
						if (nameFound)
						{
							this.Error("no positional argument after named argument");
						}
					}
					else
					{
						base.SynErr(128);
					}
					this.Expr(out expr);
					if (expr != null)
					{
						if (name == "")
						{
							positional.Add(expr);
						}
						else
						{
							named.Add(new NamedArgumentExpression(name, expr));
							name = "";
						}
					}
				}
			}
			base.Expect(21);
		}

		private void Expr(out Expression expr)
		{
			expr = null;
			Expression expr2 = null;
			Expression expr3 = null;
			this.UnaryExpr(out expr);
			if (this.StartOf(6))
			{
				AssignmentOperatorType op;
				this.AssignmentOperator(out op);
				this.Expr(out expr2);
				expr = new AssignmentExpression(expr, op, expr2);
			}
			else if (this.la.kind == 22 && this.Peek(1).kind == 35)
			{
				AssignmentOperatorType op;
				this.AssignmentOperator(out op);
				this.Expr(out expr2);
				expr = new AssignmentExpression(expr, op, expr2);
			}
			else if (this.StartOf(7))
			{
				this.ConditionalOrExpr(ref expr);
				if (this.la.kind == 13)
				{
					this.lexer.NextToken();
					this.Expr(out expr2);
					expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr2);
				}
				if (this.la.kind == 12)
				{
					this.lexer.NextToken();
					this.Expr(out expr2);
					base.Expect(9);
					this.Expr(out expr3);
					expr = new ConditionalExpression(expr, expr2, expr3);
				}
			}
			else
			{
				base.SynErr(129);
			}
		}

		private void AttributeSection(out AttributeSection section)
		{
			string attributeTarget = "";
			List<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute> attributes = new List<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute>();
			base.Expect(18);
			Location startPos = this.t.Location;
			if (this.IsLocalAttrTarget())
			{
				if (this.la.kind == 68)
				{
					this.lexer.NextToken();
					attributeTarget = "event";
				}
				else if (this.la.kind == 100)
				{
					this.lexer.NextToken();
					attributeTarget = "return";
				}
				else
				{
					this.lexer.NextToken();
					if (this.t.val != "field" || this.t.val != "method" || this.t.val != "module" || this.t.val != "param" || this.t.val != "property" || this.t.val != "type")
					{
						this.Error("attribute target specifier (event, return, field,method, module, param, property, or type) expected");
					}
					attributeTarget = this.t.val;
				}
				base.Expect(9);
			}
			AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute;
			this.Attribute(out attribute);
			attributes.Add(attribute);
			while (this.NotFinalComma())
			{
				base.Expect(14);
				this.Attribute(out attribute);
				attributes.Add(attribute);
			}
			if (this.la.kind == 14)
			{
				this.lexer.NextToken();
			}
			base.Expect(19);
			section = new AttributeSection(attributeTarget, attributes);
			section.StartLocation = startPos;
			section.EndLocation = this.t.EndLocation;
		}

		private void TypeModifier(ModifierList m)
		{
			int kind = this.la.kind;
			if (kind <= 88)
			{
				if (kind <= 48)
				{
					if (kind == 1)
					{
						this.lexer.NextToken();
						if (this.t.val == "partial")
						{
							m.Add(Modifiers.Partial, this.t.Location);
						}
						else
						{
							this.Error("Unexpected identifier");
						}
						return;
					}
					if (kind == 48)
					{
						this.lexer.NextToken();
						m.Add(Modifiers.Dim, this.t.Location);
						return;
					}
				}
				else
				{
					if (kind == 83)
					{
						this.lexer.NextToken();
						m.Add(Modifiers.Internal, this.t.Location);
						return;
					}
					if (kind == 88)
					{
						this.lexer.NextToken();
						m.Add(Modifiers.New, this.t.Location);
						return;
					}
				}
			}
			else if (kind <= 102)
			{
				switch (kind)
				{
				case 95:
					this.lexer.NextToken();
					m.Add(Modifiers.Private, this.t.Location);
					return;
				case 96:
					this.lexer.NextToken();
					m.Add(Modifiers.Protected, this.t.Location);
					return;
				case 97:
					this.lexer.NextToken();
					m.Add(Modifiers.Public, this.t.Location);
					return;
				default:
					if (kind == 102)
					{
						this.lexer.NextToken();
						m.Add(Modifiers.Sealed, this.t.Location);
						return;
					}
					break;
				}
			}
			else
			{
				if (kind == 106)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Static, this.t.Location);
					return;
				}
				if (kind == 118)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Unsafe, this.t.Location);
					return;
				}
			}
			base.SynErr(130);
		}

		private void TypeDecl(ModifierList m, List<AttributeSection> attributes)
		{
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			if (this.la.kind == 58)
			{
				m.Check(Modifiers.Classes);
				this.lexer.NextToken();
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				List<TemplateDefinition> templates = newType.Templates;
				this.compilationUnit.AddChild(newType);
				this.compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(this.t.Location);
				newType.Type = AIMS.Libraries.Scripting.NRefactory.Ast.ClassType.Class;
				base.Expect(1);
				newType.Name = this.t.val;
				if (this.la.kind == 23)
				{
					this.TypeParameterList(templates);
				}
				if (this.la.kind == 9)
				{
					List<TypeReference> names;
					this.ClassBase(out names);
					newType.BaseTypes = names;
				}
				while (this.IdentIsWhere())
				{
					this.TypeParameterConstraintsClause(templates);
				}
				newType.BodyStartLocation = this.t.EndLocation;
				base.Expect(16);
				this.ClassBody();
				base.Expect(17);
				if (this.la.kind == 11)
				{
					this.lexer.NextToken();
				}
				newType.EndLocation = this.t.Location;
				this.compilationUnit.BlockEnd();
			}
			else if (this.StartOf(8))
			{
				m.Check(Modifiers.StructsInterfacesEnumsDelegates);
				if (this.la.kind == 108)
				{
					this.lexer.NextToken();
					TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
					List<TemplateDefinition> templates = newType.Templates;
					newType.StartLocation = m.GetDeclarationLocation(this.t.Location);
					this.compilationUnit.AddChild(newType);
					this.compilationUnit.BlockStart(newType);
					newType.Type = AIMS.Libraries.Scripting.NRefactory.Ast.ClassType.Struct;
					base.Expect(1);
					newType.Name = this.t.val;
					if (this.la.kind == 23)
					{
						this.TypeParameterList(templates);
					}
					if (this.la.kind == 9)
					{
						List<TypeReference> names;
						this.StructInterfaces(out names);
						newType.BaseTypes = names;
					}
					while (this.IdentIsWhere())
					{
						this.TypeParameterConstraintsClause(templates);
					}
					newType.BodyStartLocation = this.t.EndLocation;
					this.StructBody();
					if (this.la.kind == 11)
					{
						this.lexer.NextToken();
					}
					newType.EndLocation = this.t.Location;
					this.compilationUnit.BlockEnd();
				}
				else if (this.la.kind == 82)
				{
					this.lexer.NextToken();
					TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
					List<TemplateDefinition> templates = newType.Templates;
					this.compilationUnit.AddChild(newType);
					this.compilationUnit.BlockStart(newType);
					newType.StartLocation = m.GetDeclarationLocation(this.t.Location);
					newType.Type = AIMS.Libraries.Scripting.NRefactory.Ast.ClassType.Interface;
					base.Expect(1);
					newType.Name = this.t.val;
					if (this.la.kind == 23)
					{
						this.TypeParameterList(templates);
					}
					if (this.la.kind == 9)
					{
						List<TypeReference> names;
						this.InterfaceBase(out names);
						newType.BaseTypes = names;
					}
					while (this.IdentIsWhere())
					{
						this.TypeParameterConstraintsClause(templates);
					}
					newType.BodyStartLocation = this.t.EndLocation;
					this.InterfaceBody();
					if (this.la.kind == 11)
					{
						this.lexer.NextToken();
					}
					newType.EndLocation = this.t.Location;
					this.compilationUnit.BlockEnd();
				}
				else if (this.la.kind == 67)
				{
					this.lexer.NextToken();
					TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
					this.compilationUnit.AddChild(newType);
					this.compilationUnit.BlockStart(newType);
					newType.StartLocation = m.GetDeclarationLocation(this.t.Location);
					newType.Type = AIMS.Libraries.Scripting.NRefactory.Ast.ClassType.Enum;
					base.Expect(1);
					newType.Name = this.t.val;
					if (this.la.kind == 9)
					{
						this.lexer.NextToken();
						string name;
						this.IntegralType(out name);
						newType.BaseTypes.Add(new TypeReference(name));
					}
					newType.BodyStartLocation = this.t.EndLocation;
					this.EnumBody();
					if (this.la.kind == 11)
					{
						this.lexer.NextToken();
					}
					newType.EndLocation = this.t.Location;
					this.compilationUnit.BlockEnd();
				}
				else
				{
					this.lexer.NextToken();
					DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
					List<TemplateDefinition> templates = delegateDeclr.Templates;
					delegateDeclr.StartLocation = m.GetDeclarationLocation(this.t.Location);
					if (this.NotVoidPointer())
					{
						base.Expect(122);
						delegateDeclr.ReturnType = new TypeReference("void", 0, null);
					}
					else if (this.StartOf(9))
					{
						TypeReference type;
						this.Type(out type);
						delegateDeclr.ReturnType = type;
					}
					else
					{
						base.SynErr(131);
					}
					base.Expect(1);
					delegateDeclr.Name = this.t.val;
					if (this.la.kind == 23)
					{
						this.TypeParameterList(templates);
					}
					base.Expect(20);
					if (this.StartOf(10))
					{
						this.FormalParameterList(p);
						delegateDeclr.Parameters = p;
					}
					base.Expect(21);
					while (this.IdentIsWhere())
					{
						this.TypeParameterConstraintsClause(templates);
					}
					base.Expect(11);
					delegateDeclr.EndLocation = this.t.Location;
					this.compilationUnit.AddChild(delegateDeclr);
				}
			}
			else
			{
				base.SynErr(132);
			}
		}

		private void TypeParameterList(List<TemplateDefinition> templates)
		{
			List<AttributeSection> attributes = new List<AttributeSection>();
			base.Expect(23);
			while (this.la.kind == 18)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			base.Expect(1);
			templates.Add(new TemplateDefinition(this.t.val, attributes));
			while (this.la.kind == 14)
			{
				this.lexer.NextToken();
				while (this.la.kind == 18)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				base.Expect(1);
				templates.Add(new TemplateDefinition(this.t.val, attributes));
			}
			base.Expect(22);
		}

		private void ClassBase(out List<TypeReference> names)
		{
			names = new List<TypeReference>();
			base.Expect(9);
			TypeReference typeRef;
			this.ClassType(out typeRef, false);
			if (typeRef != null)
			{
				names.Add(typeRef);
			}
			while (this.la.kind == 14)
			{
				this.lexer.NextToken();
				this.TypeName(out typeRef, false);
				if (typeRef != null)
				{
					names.Add(typeRef);
				}
			}
		}

		private void TypeParameterConstraintsClause(List<TemplateDefinition> templates)
		{
			string name = "";
			base.Expect(1);
			if (this.t.val != "where")
			{
				this.Error("where expected");
			}
			base.Expect(1);
			name = this.t.val;
			base.Expect(9);
			TypeReference type;
			this.TypeParameterConstraintsClauseBase(out type);
			TemplateDefinition td = null;
			foreach (TemplateDefinition d in templates)
			{
				if (d.Name == name)
				{
					td = d;
					break;
				}
			}
			if (td != null && type != null)
			{
				td.Bases.Add(type);
			}
			while (this.la.kind == 14)
			{
				this.lexer.NextToken();
				this.TypeParameterConstraintsClauseBase(out type);
				td = null;
				foreach (TemplateDefinition d in templates)
				{
					if (d.Name == name)
					{
						td = d;
						break;
					}
				}
				if (td != null && type != null)
				{
					td.Bases.Add(type);
				}
			}
		}

		private void ClassBody()
		{
			while (this.StartOf(11))
			{
				List<AttributeSection> attributes = new List<AttributeSection>();
				ModifierList i = new ModifierList();
				while (this.la.kind == 18)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				this.MemberModifiers(i);
				this.ClassMemberDecl(i, attributes);
			}
		}

		private void StructInterfaces(out List<TypeReference> names)
		{
			names = new List<TypeReference>();
			base.Expect(9);
			TypeReference typeRef;
			this.TypeName(out typeRef, false);
			if (typeRef != null)
			{
				names.Add(typeRef);
			}
			while (this.la.kind == 14)
			{
				this.lexer.NextToken();
				this.TypeName(out typeRef, false);
				if (typeRef != null)
				{
					names.Add(typeRef);
				}
			}
		}

		private void StructBody()
		{
			base.Expect(16);
			while (this.StartOf(12))
			{
				List<AttributeSection> attributes = new List<AttributeSection>();
				ModifierList i = new ModifierList();
				while (this.la.kind == 18)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				this.MemberModifiers(i);
				this.StructMemberDecl(i, attributes);
			}
			base.Expect(17);
		}

		private void InterfaceBase(out List<TypeReference> names)
		{
			names = new List<TypeReference>();
			base.Expect(9);
			TypeReference typeRef;
			this.TypeName(out typeRef, false);
			if (typeRef != null)
			{
				names.Add(typeRef);
			}
			while (this.la.kind == 14)
			{
				this.lexer.NextToken();
				this.TypeName(out typeRef, false);
				if (typeRef != null)
				{
					names.Add(typeRef);
				}
			}
		}

		private void InterfaceBody()
		{
			base.Expect(16);
			while (this.StartOf(13))
			{
				this.InterfaceMemberDecl();
			}
			base.Expect(17);
		}

		private void IntegralType(out string name)
		{
			name = "";
			int kind = this.la.kind;
			if (kind <= 81)
			{
				if (kind == 53)
				{
					this.lexer.NextToken();
					name = "byte";
					return;
				}
				if (kind == 56)
				{
					this.lexer.NextToken();
					name = "char";
					return;
				}
				if (kind == 81)
				{
					this.lexer.NextToken();
					name = "int";
					return;
				}
			}
			else
			{
				if (kind == 86)
				{
					this.lexer.NextToken();
					name = "long";
					return;
				}
				switch (kind)
				{
				case 101:
					this.lexer.NextToken();
					name = "sbyte";
					return;
				case 102:
					break;
				case 103:
					this.lexer.NextToken();
					name = "short";
					return;
				default:
					switch (kind)
					{
					case 115:
						this.lexer.NextToken();
						name = "uint";
						return;
					case 116:
						this.lexer.NextToken();
						name = "ulong";
						return;
					case 119:
						this.lexer.NextToken();
						name = "ushort";
						return;
					}
					break;
				}
			}
			base.SynErr(133);
		}

		private void EnumBody()
		{
			base.Expect(16);
			if (this.la.kind == 1 || this.la.kind == 18)
			{
				FieldDeclaration f;
				this.EnumMemberDecl(out f);
				this.compilationUnit.AddChild(f);
				while (this.NotFinalComma())
				{
					base.Expect(14);
					this.EnumMemberDecl(out f);
					this.compilationUnit.AddChild(f);
				}
				if (this.la.kind == 14)
				{
					this.lexer.NextToken();
				}
			}
			base.Expect(17);
		}

		private void Type(out TypeReference type)
		{
			this.TypeWithRestriction(out type, true, false);
		}

		private void FormalParameterList(List<ParameterDeclarationExpression> parameter)
		{
			List<AttributeSection> attributes = new List<AttributeSection>();
			while (this.la.kind == 18)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			if (this.StartOf(14))
			{
				ParameterDeclarationExpression p;
				this.FixedParameter(out p);
				bool paramsFound = false;
				p.Attributes = attributes;
				parameter.Add(p);
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					attributes = new List<AttributeSection>();
					if (paramsFound)
					{
						this.Error("params array must be at end of parameter list");
					}
					while (this.la.kind == 18)
					{
						AttributeSection section;
						this.AttributeSection(out section);
						attributes.Add(section);
					}
					if (this.StartOf(14))
					{
						this.FixedParameter(out p);
						p.Attributes = attributes;
						parameter.Add(p);
					}
					else if (this.la.kind == 94)
					{
						this.ParameterArray(out p);
						paramsFound = true;
						p.Attributes = attributes;
						parameter.Add(p);
					}
					else
					{
						base.SynErr(134);
					}
				}
			}
			else if (this.la.kind == 94)
			{
				ParameterDeclarationExpression p;
				this.ParameterArray(out p);
				p.Attributes = attributes;
				parameter.Add(p);
			}
			else
			{
				base.SynErr(135);
			}
		}

		private void ClassType(out TypeReference typeRef, bool canBeUnbound)
		{
			typeRef = null;
			if (this.la.kind == 1)
			{
				TypeReference r;
				this.TypeName(out r, canBeUnbound);
				typeRef = r;
			}
			else if (this.la.kind == 90)
			{
				this.lexer.NextToken();
				typeRef = new TypeReference("object");
			}
			else if (this.la.kind == 107)
			{
				this.lexer.NextToken();
				typeRef = new TypeReference("string");
			}
			else
			{
				base.SynErr(136);
			}
		}

		private void TypeName(out TypeReference typeRef, bool canBeUnbound)
		{
			List<TypeReference> typeArguments = null;
			string alias = null;
			if (this.la.kind == 1 && this.Peek(1).kind == 10)
			{
				this.lexer.NextToken();
				alias = this.t.val;
				base.Expect(10);
			}
			string qualident;
			this.Qualident(out qualident);
			if (this.la.kind == 23)
			{
				this.TypeArgumentList(out typeArguments, canBeUnbound);
			}
			if (alias == null)
			{
				typeRef = new TypeReference(qualident, typeArguments);
			}
			else if (alias == "global")
			{
				typeRef = new TypeReference(qualident, typeArguments);
				typeRef.IsGlobal = true;
			}
			else
			{
				typeRef = new TypeReference(alias + "." + qualident, typeArguments);
			}
			while (this.DotAndIdent())
			{
				base.Expect(15);
				typeArguments = null;
				this.Qualident(out qualident);
				if (this.la.kind == 23)
				{
					this.TypeArgumentList(out typeArguments, canBeUnbound);
				}
				typeRef = new InnerClassTypeReference(typeRef, qualident, typeArguments);
			}
		}

		private void MemberModifiers(ModifierList m)
		{
			while (this.StartOf(15) || (this.la.kind == 1 && this.la.val == "partial"))
			{
				if (this.la.kind == 48)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Dim, this.t.Location);
				}
				else if (this.la.kind == 70)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Extern, this.t.Location);
				}
				else if (this.la.kind == 83)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Internal, this.t.Location);
				}
				else if (this.la.kind == 88)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.New, this.t.Location);
				}
				else if (this.la.kind == 93)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Override, this.t.Location);
				}
				else if (this.la.kind == 95)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Private, this.t.Location);
				}
				else if (this.la.kind == 96)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Protected, this.t.Location);
				}
				else if (this.la.kind == 97)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Public, this.t.Location);
				}
				else if (this.la.kind == 98)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.ReadOnly, this.t.Location);
				}
				else if (this.la.kind == 102)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Sealed, this.t.Location);
				}
				else if (this.la.kind == 106)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Static, this.t.Location);
				}
				else if (this.la.kind == 73)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Fixed, this.t.Location);
				}
				else if (this.la.kind == 118)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Unsafe, this.t.Location);
				}
				else if (this.la.kind == 121)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Virtual, this.t.Location);
				}
				else if (this.la.kind == 123)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Volatile, this.t.Location);
				}
				else
				{
					base.Expect(1);
					m.Add(Modifiers.Partial, this.t.Location);
				}
			}
		}

		private void ClassMemberDecl(ModifierList m, List<AttributeSection> attributes)
		{
			Statement stmt = null;
			if (this.StartOf(16))
			{
				this.StructMemberDecl(m, attributes);
			}
			else if (this.la.kind == 27)
			{
				m.Check(Modifiers.Destructors);
				Location startPos = this.t.Location;
				this.lexer.NextToken();
				base.Expect(1);
				DestructorDeclaration d = new DestructorDeclaration(this.t.val, m.Modifier, attributes);
				d.Modifier = m.Modifier;
				d.StartLocation = m.GetDeclarationLocation(startPos);
				base.Expect(20);
				base.Expect(21);
				d.EndLocation = this.t.EndLocation;
				if (this.la.kind == 16)
				{
					this.Block(out stmt);
				}
				else if (this.la.kind == 11)
				{
					this.lexer.NextToken();
				}
				else
				{
					base.SynErr(137);
				}
				d.Body = (BlockStatement)stmt;
				this.compilationUnit.AddChild(d);
			}
			else
			{
				base.SynErr(138);
			}
		}

		private void StructMemberDecl(ModifierList m, List<AttributeSection> attributes)
		{
			string qualident = null;
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			Statement stmt = null;
			List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
			List<TemplateDefinition> templates = new List<TemplateDefinition>();
			TypeReference explicitInterface = null;
			if (this.la.kind == 59)
			{
				m.Check(Modifiers.VBStructures);
				this.lexer.NextToken();
				Location startPos = this.t.Location;
				TypeReference type;
				this.Type(out type);
				base.Expect(1);
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifiers.Const);
				fd.StartLocation = m.GetDeclarationLocation(startPos);
				VariableDeclaration f = new VariableDeclaration(this.t.val);
				fd.Fields.Add(f);
				base.Expect(3);
				Expression expr;
				this.Expr(out expr);
				f.Initializer = expr;
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					base.Expect(1);
					f = new VariableDeclaration(this.t.val);
					fd.Fields.Add(f);
					base.Expect(3);
					this.Expr(out expr);
					f.Initializer = expr;
				}
				base.Expect(11);
				fd.EndLocation = this.t.EndLocation;
				this.compilationUnit.AddChild(fd);
			}
			else if (this.NotVoidPointer())
			{
				m.Check(Modifiers.PropertysEventsMethods);
				base.Expect(122);
				Location startPos = this.t.Location;
				if (this.IsExplicitInterfaceImplementation())
				{
					this.TypeName(out explicitInterface, false);
					if (this.la.kind != 15 || this.Peek(1).kind != 110)
					{
						qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					}
				}
				else if (this.la.kind == 1)
				{
					this.lexer.NextToken();
					qualident = this.t.val;
				}
				else
				{
					base.SynErr(139);
				}
				if (this.la.kind == 23)
				{
					this.TypeParameterList(templates);
				}
				base.Expect(20);
				if (this.StartOf(10))
				{
					this.FormalParameterList(p);
				}
				base.Expect(21);
				MethodDeclaration methodDeclaration = new MethodDeclaration(qualident, m.Modifier, new TypeReference("void"), p, attributes);
				methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				methodDeclaration.EndLocation = this.t.EndLocation;
				methodDeclaration.Templates = templates;
				if (explicitInterface != null)
				{
					methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
				}
				this.compilationUnit.AddChild(methodDeclaration);
				this.compilationUnit.BlockStart(methodDeclaration);
				while (this.IdentIsWhere())
				{
					this.TypeParameterConstraintsClause(templates);
				}
				if (this.la.kind == 16)
				{
					this.Block(out stmt);
				}
				else if (this.la.kind == 11)
				{
					this.lexer.NextToken();
				}
				else
				{
					base.SynErr(140);
				}
				this.compilationUnit.BlockEnd();
				methodDeclaration.Body = (BlockStatement)stmt;
			}
			else if (this.la.kind == 68)
			{
				m.Check(Modifiers.PropertysEventsMethods);
				this.lexer.NextToken();
				EventDeclaration eventDecl = new EventDeclaration(null, null, m.Modifier, attributes, null);
				eventDecl.StartLocation = this.t.Location;
				this.compilationUnit.AddChild(eventDecl);
				this.compilationUnit.BlockStart(eventDecl);
				EventAddRegion addBlock = null;
				EventRemoveRegion removeBlock = null;
				TypeReference type;
				this.Type(out type);
				eventDecl.TypeReference = type;
				if (this.IsExplicitInterfaceImplementation())
				{
					this.TypeName(out explicitInterface, false);
					qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					eventDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
				}
				else if (this.la.kind == 1)
				{
					this.lexer.NextToken();
					qualident = this.t.val;
				}
				else
				{
					base.SynErr(141);
				}
				eventDecl.Name = qualident;
				eventDecl.EndLocation = this.t.EndLocation;
				if (this.la.kind == 3)
				{
					this.lexer.NextToken();
					Expression expr;
					this.Expr(out expr);
					eventDecl.Initializer = expr;
				}
				if (this.la.kind == 16)
				{
					this.lexer.NextToken();
					eventDecl.BodyStart = this.t.Location;
					this.EventAccessorDecls(out addBlock, out removeBlock);
					base.Expect(17);
					eventDecl.BodyEnd = this.t.EndLocation;
				}
				if (this.la.kind == 11)
				{
					this.lexer.NextToken();
				}
				this.compilationUnit.BlockEnd();
				eventDecl.AddRegion = addBlock;
				eventDecl.RemoveRegion = removeBlock;
			}
			else if (this.IdentAndLPar())
			{
				m.Check(Modifiers.Private | Modifiers.Internal | Modifiers.Protected | Modifiers.Public | Modifiers.Static | Modifiers.Extern | Modifiers.Unsafe);
				base.Expect(1);
				string name = this.t.val;
				Location startPos = this.t.Location;
				base.Expect(20);
				if (this.StartOf(10))
				{
					m.Check(Modifiers.Constructors);
					this.FormalParameterList(p);
				}
				base.Expect(21);
				ConstructorInitializer init = null;
				if (this.la.kind == 9)
				{
					m.Check(Modifiers.Constructors);
					this.ConstructorInitializer(out init);
				}
				ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes);
				cd.StartLocation = startPos;
				cd.EndLocation = this.t.EndLocation;
				if (this.la.kind == 16)
				{
					this.Block(out stmt);
				}
				else if (this.la.kind == 11)
				{
					this.lexer.NextToken();
				}
				else
				{
					base.SynErr(142);
				}
				cd.Body = (BlockStatement)stmt;
				this.compilationUnit.AddChild(cd);
			}
			else if (this.la.kind == 69 || this.la.kind == 79)
			{
				m.Check(Modifiers.Operators);
				if (m.isNone)
				{
					this.Error("at least one modifier must be set");
				}
				bool isImplicit = true;
				Location startPos = Location.Empty;
				if (this.la.kind == 79)
				{
					this.lexer.NextToken();
					startPos = this.t.Location;
				}
				else
				{
					this.lexer.NextToken();
					isImplicit = false;
					startPos = this.t.Location;
				}
				base.Expect(91);
				TypeReference type;
				this.Type(out type);
				TypeReference operatorType = type;
				base.Expect(20);
				this.Type(out type);
				base.Expect(1);
				string varName = this.t.val;
				base.Expect(21);
				Location endPos = this.t.Location;
				if (this.la.kind == 16)
				{
					this.Block(out stmt);
				}
				else if (this.la.kind == 11)
				{
					this.lexer.NextToken();
					stmt = null;
				}
				else
				{
					base.SynErr(143);
				}
				List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
				parameters.Add(new ParameterDeclarationExpression(type, varName));
				OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier, attributes, parameters, operatorType, isImplicit ? ConversionType.Implicit : ConversionType.Explicit);
				operatorDeclaration.Body = (BlockStatement)stmt;
				operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				operatorDeclaration.EndLocation = endPos;
				this.compilationUnit.AddChild(operatorDeclaration);
			}
			else if (this.StartOf(17))
			{
				this.TypeDecl(m, attributes);
			}
			else if (this.StartOf(9))
			{
				TypeReference type;
				this.Type(out type);
				Location startPos = this.t.Location;
				if (this.la.kind == 91)
				{
					m.Check(Modifiers.Operators);
					if (m.isNone)
					{
						this.Error("at least one modifier must be set");
					}
					this.lexer.NextToken();
					OverloadableOperatorType op;
					this.OverloadableOperator(out op);
					TypeReference secondType = null;
					string secondName = null;
					base.Expect(20);
					TypeReference firstType;
					this.Type(out firstType);
					base.Expect(1);
					string firstName = this.t.val;
					if (this.la.kind == 14)
					{
						this.lexer.NextToken();
						this.Type(out secondType);
						base.Expect(1);
						secondName = this.t.val;
					}
					else if (this.la.kind != 21)
					{
						base.SynErr(144);
					}
					Location endPos = this.t.Location;
					base.Expect(21);
					if (this.la.kind == 16)
					{
						this.Block(out stmt);
					}
					else if (this.la.kind == 11)
					{
						this.lexer.NextToken();
					}
					else
					{
						base.SynErr(145);
					}
					List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
					parameters.Add(new ParameterDeclarationExpression(firstType, firstName));
					if (secondType != null)
					{
						parameters.Add(new ParameterDeclarationExpression(secondType, secondName));
					}
					OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier, attributes, parameters, type, op);
					operatorDeclaration.Body = (BlockStatement)stmt;
					operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
					operatorDeclaration.EndLocation = endPos;
					this.compilationUnit.AddChild(operatorDeclaration);
				}
				else if (this.IsVarDecl())
				{
					m.Check(Modifiers.Fields);
					FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
					fd.StartLocation = m.GetDeclarationLocation(startPos);
					if (m.Contains(Modifiers.Fixed))
					{
						this.VariableDeclarator(variableDeclarators);
						base.Expect(18);
						Expression expr;
						this.Expr(out expr);
						if (variableDeclarators.Count > 0)
						{
							variableDeclarators[variableDeclarators.Count - 1].FixedArrayInitialization = expr;
						}
						base.Expect(19);
						while (this.la.kind == 14)
						{
							this.lexer.NextToken();
							this.VariableDeclarator(variableDeclarators);
							base.Expect(18);
							this.Expr(out expr);
							if (variableDeclarators.Count > 0)
							{
								variableDeclarators[variableDeclarators.Count - 1].FixedArrayInitialization = expr;
							}
							base.Expect(19);
						}
					}
					else if (this.la.kind == 1)
					{
						this.VariableDeclarator(variableDeclarators);
						while (this.la.kind == 14)
						{
							this.lexer.NextToken();
							this.VariableDeclarator(variableDeclarators);
						}
					}
					else
					{
						base.SynErr(146);
					}
					base.Expect(11);
					fd.EndLocation = this.t.EndLocation;
					fd.Fields = variableDeclarators;
					this.compilationUnit.AddChild(fd);
				}
				else if (this.la.kind == 110)
				{
					m.Check(Modifiers.Indexers);
					this.lexer.NextToken();
					base.Expect(18);
					this.FormalParameterList(p);
					base.Expect(19);
					Location endLocation = this.t.EndLocation;
					base.Expect(16);
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = startPos;
					indexer.EndLocation = endLocation;
					indexer.BodyStart = this.t.Location;
					PropertyGetRegion getRegion;
					PropertySetRegion setRegion;
					this.AccessorDecls(out getRegion, out setRegion);
					base.Expect(17);
					indexer.BodyEnd = this.t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					this.compilationUnit.AddChild(indexer);
				}
				else if (this.la.kind == 1)
				{
					if (this.IsExplicitInterfaceImplementation())
					{
						this.TypeName(out explicitInterface, false);
						if (this.la.kind != 15 || this.Peek(1).kind != 110)
						{
							qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
						}
					}
					else if (this.la.kind == 1)
					{
						this.lexer.NextToken();
						qualident = this.t.val;
					}
					else
					{
						base.SynErr(147);
					}
					Location qualIdentEndLocation = this.t.EndLocation;
					if (this.la.kind == 16 || this.la.kind == 20 || this.la.kind == 23)
					{
						if (this.la.kind == 20 || this.la.kind == 23)
						{
							m.Check(Modifiers.PropertysEventsMethods);
							if (this.la.kind == 23)
							{
								this.TypeParameterList(templates);
							}
							base.Expect(20);
							if (this.StartOf(10))
							{
								this.FormalParameterList(p);
							}
							base.Expect(21);
							MethodDeclaration methodDeclaration = new MethodDeclaration(qualident, m.Modifier, type, p, attributes);
							if (explicitInterface != null)
							{
								methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
							}
							methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
							methodDeclaration.EndLocation = this.t.EndLocation;
							methodDeclaration.Templates = templates;
							this.compilationUnit.AddChild(methodDeclaration);
							while (this.IdentIsWhere())
							{
								this.TypeParameterConstraintsClause(templates);
							}
							if (this.la.kind == 16)
							{
								this.Block(out stmt);
							}
							else if (this.la.kind == 11)
							{
								this.lexer.NextToken();
							}
							else
							{
								base.SynErr(148);
							}
							methodDeclaration.Body = (BlockStatement)stmt;
						}
						else
						{
							this.lexer.NextToken();
							PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes);
							if (explicitInterface != null)
							{
								pDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
							}
							pDecl.StartLocation = m.GetDeclarationLocation(startPos);
							pDecl.EndLocation = qualIdentEndLocation;
							pDecl.BodyStart = this.t.Location;
							PropertyGetRegion getRegion;
							PropertySetRegion setRegion;
							this.AccessorDecls(out getRegion, out setRegion);
							base.Expect(17);
							pDecl.GetRegion = getRegion;
							pDecl.SetRegion = setRegion;
							pDecl.BodyEnd = this.t.EndLocation;
							this.compilationUnit.AddChild(pDecl);
						}
					}
					else if (this.la.kind == 15)
					{
						m.Check(Modifiers.Indexers);
						this.lexer.NextToken();
						base.Expect(110);
						base.Expect(18);
						this.FormalParameterList(p);
						base.Expect(19);
						IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
						indexer.StartLocation = m.GetDeclarationLocation(startPos);
						indexer.EndLocation = this.t.EndLocation;
						if (explicitInterface != null)
						{
							indexer.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, "this"));
						}
						base.Expect(16);
						Location bodyStart = this.t.Location;
						PropertyGetRegion getRegion;
						PropertySetRegion setRegion;
						this.AccessorDecls(out getRegion, out setRegion);
						base.Expect(17);
						indexer.BodyStart = bodyStart;
						indexer.BodyEnd = this.t.EndLocation;
						indexer.GetRegion = getRegion;
						indexer.SetRegion = setRegion;
						this.compilationUnit.AddChild(indexer);
					}
					else
					{
						base.SynErr(149);
					}
				}
				else
				{
					base.SynErr(150);
				}
			}
			else
			{
				base.SynErr(151);
			}
		}

		private void InterfaceMemberDecl()
		{
			Modifiers mod = Modifiers.None;
			List<AttributeSection> attributes = new List<AttributeSection>();
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			Location startLocation = new Location(-1, -1);
			List<TemplateDefinition> templates = new List<TemplateDefinition>();
			while (this.la.kind == 18)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			if (this.la.kind == 88)
			{
				this.lexer.NextToken();
				mod = Modifiers.New;
				startLocation = this.t.Location;
			}
			if (this.NotVoidPointer())
			{
				base.Expect(122);
				if (startLocation.X == -1)
				{
					startLocation = this.t.Location;
				}
				base.Expect(1);
				string name = this.t.val;
				if (this.la.kind == 23)
				{
					this.TypeParameterList(templates);
				}
				base.Expect(20);
				if (this.StartOf(10))
				{
					this.FormalParameterList(parameters);
				}
				base.Expect(21);
				while (this.IdentIsWhere())
				{
					this.TypeParameterConstraintsClause(templates);
				}
				base.Expect(11);
				MethodDeclaration md = new MethodDeclaration(name, mod, new TypeReference("void"), parameters, attributes);
				md.StartLocation = startLocation;
				md.EndLocation = this.t.EndLocation;
				md.Templates = templates;
				this.compilationUnit.AddChild(md);
			}
			else if (this.StartOf(18))
			{
				if (this.StartOf(9))
				{
					TypeReference type;
					this.Type(out type);
					if (startLocation.X == -1)
					{
						startLocation = this.t.Location;
					}
					if (this.la.kind == 1)
					{
						this.lexer.NextToken();
						string name = this.t.val;
						Location qualIdentEndLocation = this.t.EndLocation;
						if (this.la.kind == 20 || this.la.kind == 23)
						{
							if (this.la.kind == 23)
							{
								this.TypeParameterList(templates);
							}
							base.Expect(20);
							if (this.StartOf(10))
							{
								this.FormalParameterList(parameters);
							}
							base.Expect(21);
							while (this.IdentIsWhere())
							{
								this.TypeParameterConstraintsClause(templates);
							}
							base.Expect(11);
							MethodDeclaration md = new MethodDeclaration(name, mod, type, parameters, attributes);
							md.StartLocation = startLocation;
							md.EndLocation = this.t.EndLocation;
							md.Templates = templates;
							this.compilationUnit.AddChild(md);
						}
						else if (this.la.kind == 16)
						{
							PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes);
							this.compilationUnit.AddChild(pd);
							this.lexer.NextToken();
							Location bodyStart = this.t.Location;
							PropertyGetRegion getBlock;
							PropertySetRegion setBlock;
							this.InterfaceAccessors(out getBlock, out setBlock);
							base.Expect(17);
							pd.GetRegion = getBlock;
							pd.SetRegion = setBlock;
							pd.StartLocation = startLocation;
							pd.EndLocation = qualIdentEndLocation;
							pd.BodyStart = bodyStart;
							pd.BodyEnd = this.t.EndLocation;
						}
						else
						{
							base.SynErr(152);
						}
					}
					else if (this.la.kind == 110)
					{
						this.lexer.NextToken();
						base.Expect(18);
						this.FormalParameterList(parameters);
						base.Expect(19);
						Location bracketEndLocation = this.t.EndLocation;
						IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes);
						this.compilationUnit.AddChild(id);
						base.Expect(16);
						Location bodyStart = this.t.Location;
						PropertyGetRegion getBlock;
						PropertySetRegion setBlock;
						this.InterfaceAccessors(out getBlock, out setBlock);
						base.Expect(17);
						id.GetRegion = getBlock;
						id.SetRegion = setBlock;
						id.StartLocation = startLocation;
						id.EndLocation = bracketEndLocation;
						id.BodyStart = bodyStart;
						id.BodyEnd = this.t.EndLocation;
					}
					else
					{
						base.SynErr(153);
					}
				}
				else
				{
					this.lexer.NextToken();
					if (startLocation.X == -1)
					{
						startLocation = this.t.Location;
					}
					TypeReference type;
					this.Type(out type);
					base.Expect(1);
					EventDeclaration ed = new EventDeclaration(type, this.t.val, mod, attributes, null);
					this.compilationUnit.AddChild(ed);
					base.Expect(11);
					ed.StartLocation = startLocation;
					ed.EndLocation = this.t.EndLocation;
				}
			}
			else
			{
				base.SynErr(154);
			}
		}

		private void EnumMemberDecl(out FieldDeclaration f)
		{
			Expression expr = null;
			List<AttributeSection> attributes = new List<AttributeSection>();
			AttributeSection section = null;
			while (this.la.kind == 18)
			{
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			base.Expect(1);
			f = new FieldDeclaration(attributes);
			VariableDeclaration varDecl = new VariableDeclaration(this.t.val);
			f.Fields.Add(varDecl);
			f.StartLocation = this.t.Location;
			if (this.la.kind == 3)
			{
				this.lexer.NextToken();
				this.Expr(out expr);
				varDecl.Initializer = expr;
			}
		}

		private void TypeWithRestriction(out TypeReference type, bool allowNullable, bool canBeUnbound)
		{
			int pointer = 0;
			type = null;
			if (this.la.kind == 1 || this.la.kind == 90 || this.la.kind == 107)
			{
				this.ClassType(out type, canBeUnbound);
			}
			else if (this.StartOf(4))
			{
				string name;
				this.SimpleType(out name);
				type = new TypeReference(name);
			}
			else if (this.la.kind == 122)
			{
				this.lexer.NextToken();
				base.Expect(6);
				pointer = 1;
				type = new TypeReference("void");
			}
			else
			{
				base.SynErr(155);
			}
			List<int> r = new List<int>();
			if (allowNullable && this.la.kind == 12)
			{
				this.NullableQuestionMark(ref type);
			}
			while (this.IsPointerOrDims())
			{
				int i = 0;
				if (this.la.kind == 6)
				{
					this.lexer.NextToken();
					pointer++;
				}
				else if (this.la.kind == 18)
				{
					this.lexer.NextToken();
					while (this.la.kind == 14)
					{
						this.lexer.NextToken();
						i++;
					}
					base.Expect(19);
					r.Add(i);
				}
				else
				{
					base.SynErr(156);
				}
			}
			if (type != null)
			{
				type.RankSpecifier = r.ToArray();
				type.PointerNestingLevel = pointer;
			}
		}

		private void SimpleType(out string name)
		{
			name = string.Empty;
			if (this.StartOf(19))
			{
				this.IntegralType(out name);
			}
			else if (this.la.kind == 74)
			{
				this.lexer.NextToken();
				name = "float";
			}
			else if (this.la.kind == 65)
			{
				this.lexer.NextToken();
				name = "double";
			}
			else if (this.la.kind == 61)
			{
				this.lexer.NextToken();
				name = "decimal";
			}
			else if (this.la.kind == 51)
			{
				this.lexer.NextToken();
				name = "bool";
			}
			else
			{
				base.SynErr(157);
			}
		}

		private void NullableQuestionMark(ref TypeReference typeRef)
		{
			List<TypeReference> typeArguments = new List<TypeReference>(1);
			base.Expect(12);
			if (typeRef != null)
			{
				typeArguments.Add(typeRef);
			}
			typeRef = new TypeReference("System.Nullable", typeArguments);
		}

		private void FixedParameter(out ParameterDeclarationExpression p)
		{
			ParameterModifiers mod = ParameterModifiers.In;
			Location start = this.t.Location;
			if (this.la.kind == 92 || this.la.kind == 99)
			{
				if (this.la.kind == 99)
				{
					this.lexer.NextToken();
					mod = ParameterModifiers.Ref;
				}
				else
				{
					this.lexer.NextToken();
					mod = ParameterModifiers.Out;
				}
			}
			TypeReference type;
			this.Type(out type);
			base.Expect(1);
			p = new ParameterDeclarationExpression(type, this.t.val, mod);
			p.StartLocation = start;
			p.EndLocation = this.t.Location;
		}

		private void ParameterArray(out ParameterDeclarationExpression p)
		{
			base.Expect(94);
			TypeReference type;
			this.Type(out type);
			base.Expect(1);
			p = new ParameterDeclarationExpression(type, this.t.val, ParameterModifiers.Params);
		}

		private void AccessorModifiers(out ModifierList m)
		{
			m = new ModifierList();
			if (this.la.kind == 95)
			{
				this.lexer.NextToken();
				m.Add(Modifiers.Private, this.t.Location);
			}
			else if (this.la.kind == 96)
			{
				this.lexer.NextToken();
				m.Add(Modifiers.Protected, this.t.Location);
				if (this.la.kind == 83)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Internal, this.t.Location);
				}
			}
			else if (this.la.kind == 83)
			{
				this.lexer.NextToken();
				m.Add(Modifiers.Internal, this.t.Location);
				if (this.la.kind == 96)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Protected, this.t.Location);
				}
			}
			else
			{
				base.SynErr(158);
			}
		}

		private void Block(out Statement stmt)
		{
			base.Expect(16);
			BlockStatement blockStmt = new BlockStatement();
			blockStmt.StartLocation = this.t.Location;
			this.compilationUnit.BlockStart(blockStmt);
			if (!base.ParseMethodBodies)
			{
				this.lexer.SkipCurrentBlock(0);
			}
			while (this.StartOf(20))
			{
				this.Statement();
			}
			base.Expect(17);
			stmt = blockStmt;
			blockStmt.EndLocation = this.t.EndLocation;
			this.compilationUnit.BlockEnd();
		}

		private void EventAccessorDecls(out EventAddRegion addBlock, out EventRemoveRegion removeBlock)
		{
			List<AttributeSection> attributes = new List<AttributeSection>();
			addBlock = null;
			removeBlock = null;
			while (this.la.kind == 18)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			if (this.IdentIsAdd())
			{
				addBlock = new EventAddRegion(attributes);
				Statement stmt;
				this.AddAccessorDecl(out stmt);
				attributes = new List<AttributeSection>();
				addBlock.Block = (BlockStatement)stmt;
				while (this.la.kind == 18)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				this.RemoveAccessorDecl(out stmt);
				removeBlock = new EventRemoveRegion(attributes);
				removeBlock.Block = (BlockStatement)stmt;
			}
			else if (this.IdentIsRemove())
			{
				Statement stmt;
				this.RemoveAccessorDecl(out stmt);
				removeBlock = new EventRemoveRegion(attributes);
				removeBlock.Block = (BlockStatement)stmt;
				attributes = new List<AttributeSection>();
				while (this.la.kind == 18)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				this.AddAccessorDecl(out stmt);
				addBlock = new EventAddRegion(attributes);
				addBlock.Block = (BlockStatement)stmt;
			}
			else if (this.la.kind == 1)
			{
				this.lexer.NextToken();
				this.Error("add or remove accessor declaration expected");
			}
			else
			{
				base.SynErr(159);
			}
		}

		private void ConstructorInitializer(out ConstructorInitializer ci)
		{
			ci = new ConstructorInitializer();
			base.Expect(9);
			if (this.la.kind == 50)
			{
				this.lexer.NextToken();
				ci.ConstructorInitializerType = ConstructorInitializerType.Base;
			}
			else if (this.la.kind == 110)
			{
				this.lexer.NextToken();
				ci.ConstructorInitializerType = ConstructorInitializerType.This;
			}
			else
			{
				base.SynErr(160);
			}
			base.Expect(20);
			if (this.StartOf(21))
			{
				Expression expr;
				this.Argument(out expr);
				if (expr != null)
				{
					ci.Arguments.Add(expr);
				}
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					this.Argument(out expr);
					if (expr != null)
					{
						ci.Arguments.Add(expr);
					}
				}
			}
			base.Expect(21);
		}

		private void OverloadableOperator(out OverloadableOperatorType op)
		{
			op = OverloadableOperatorType.None;
			int kind = this.la.kind;
			switch (kind)
			{
			case 4:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Add;
				return;
			case 5:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Subtract;
				return;
			case 6:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Multiply;
				return;
			case 7:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Divide;
				return;
			case 8:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Modulus;
				return;
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 25:
			case 26:
				break;
			case 22:
				this.lexer.NextToken();
				op = OverloadableOperatorType.GreaterThan;
				if (this.la.kind == 22)
				{
					this.lexer.NextToken();
					op = OverloadableOperatorType.ShiftRight;
				}
				return;
			case 23:
				this.lexer.NextToken();
				op = OverloadableOperatorType.LessThan;
				return;
			case 24:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Not;
				return;
			case 27:
				this.lexer.NextToken();
				op = OverloadableOperatorType.BitNot;
				return;
			case 28:
				this.lexer.NextToken();
				op = OverloadableOperatorType.BitwiseAnd;
				return;
			case 29:
				this.lexer.NextToken();
				op = OverloadableOperatorType.BitwiseOr;
				return;
			case 30:
				this.lexer.NextToken();
				op = OverloadableOperatorType.ExclusiveOr;
				return;
			case 31:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Increment;
				return;
			case 32:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Decrement;
				return;
			case 33:
				this.lexer.NextToken();
				op = OverloadableOperatorType.Equality;
				return;
			case 34:
				this.lexer.NextToken();
				op = OverloadableOperatorType.InEquality;
				return;
			case 35:
				this.lexer.NextToken();
				op = OverloadableOperatorType.GreaterThanOrEqual;
				return;
			case 36:
				this.lexer.NextToken();
				op = OverloadableOperatorType.LessThanOrEqual;
				return;
			case 37:
				this.lexer.NextToken();
				op = OverloadableOperatorType.ShiftLeft;
				return;
			default:
				if (kind == 71)
				{
					this.lexer.NextToken();
					op = OverloadableOperatorType.IsFalse;
					return;
				}
				if (kind == 112)
				{
					this.lexer.NextToken();
					op = OverloadableOperatorType.IsTrue;
					return;
				}
				break;
			}
			base.SynErr(161);
		}

		private void VariableDeclarator(List<VariableDeclaration> fieldDeclaration)
		{
			Expression expr = null;
			base.Expect(1);
			VariableDeclaration f = new VariableDeclaration(this.t.val);
			if (this.la.kind == 3)
			{
				this.lexer.NextToken();
				this.VariableInitializer(out expr);
				f.Initializer = expr;
			}
			fieldDeclaration.Add(f);
		}

		private void AccessorDecls(out PropertyGetRegion getBlock, out PropertySetRegion setBlock)
		{
			List<AttributeSection> attributes = new List<AttributeSection>();
			getBlock = null;
			setBlock = null;
			ModifierList modifiers = null;
			while (this.la.kind == 18)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			if (this.la.kind == 83 || this.la.kind == 95 || this.la.kind == 96)
			{
				this.AccessorModifiers(out modifiers);
			}
			if (this.IdentIsGet())
			{
				this.GetAccessorDecl(out getBlock, attributes);
				if (modifiers != null)
				{
					getBlock.Modifier = modifiers.Modifier;
				}
				if (this.StartOf(22))
				{
					attributes = new List<AttributeSection>();
					modifiers = null;
					while (this.la.kind == 18)
					{
						AttributeSection section;
						this.AttributeSection(out section);
						attributes.Add(section);
					}
					if (this.la.kind == 83 || this.la.kind == 95 || this.la.kind == 96)
					{
						this.AccessorModifiers(out modifiers);
					}
					this.SetAccessorDecl(out setBlock, attributes);
					if (modifiers != null)
					{
						setBlock.Modifier = modifiers.Modifier;
					}
				}
			}
			else if (this.IdentIsSet())
			{
				this.SetAccessorDecl(out setBlock, attributes);
				if (modifiers != null)
				{
					setBlock.Modifier = modifiers.Modifier;
				}
				if (this.StartOf(22))
				{
					attributes = new List<AttributeSection>();
					modifiers = null;
					while (this.la.kind == 18)
					{
						AttributeSection section;
						this.AttributeSection(out section);
						attributes.Add(section);
					}
					if (this.la.kind == 83 || this.la.kind == 95 || this.la.kind == 96)
					{
						this.AccessorModifiers(out modifiers);
					}
					this.GetAccessorDecl(out getBlock, attributes);
					if (modifiers != null)
					{
						getBlock.Modifier = modifiers.Modifier;
					}
				}
			}
			else if (this.la.kind == 1)
			{
				this.lexer.NextToken();
				this.Error("get or set accessor declaration expected");
			}
			else
			{
				base.SynErr(162);
			}
		}

		private void InterfaceAccessors(out PropertyGetRegion getBlock, out PropertySetRegion setBlock)
		{
			List<AttributeSection> attributes = new List<AttributeSection>();
			getBlock = null;
			setBlock = null;
			PropertyGetSetRegion lastBlock = null;
			while (this.la.kind == 18)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			Location startLocation = this.la.Location;
			if (this.IdentIsGet())
			{
				base.Expect(1);
				getBlock = new PropertyGetRegion(null, attributes);
			}
			else if (this.IdentIsSet())
			{
				base.Expect(1);
				setBlock = new PropertySetRegion(null, attributes);
			}
			else if (this.la.kind == 1)
			{
				this.lexer.NextToken();
				this.Error("set or get expected");
			}
			else
			{
				base.SynErr(163);
			}
			base.Expect(11);
			if (getBlock != null)
			{
				getBlock.StartLocation = startLocation;
				getBlock.EndLocation = this.t.EndLocation;
			}
			if (setBlock != null)
			{
				setBlock.StartLocation = startLocation;
				setBlock.EndLocation = this.t.EndLocation;
			}
			attributes = new List<AttributeSection>();
			if (this.la.kind == 1 || this.la.kind == 18)
			{
				while (this.la.kind == 18)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				startLocation = this.la.Location;
				if (this.IdentIsGet())
				{
					base.Expect(1);
					if (getBlock != null)
					{
						this.Error("get already declared");
					}
					else
					{
						getBlock = new PropertyGetRegion(null, attributes);
						lastBlock = getBlock;
					}
				}
				else if (this.IdentIsSet())
				{
					base.Expect(1);
					if (setBlock != null)
					{
						this.Error("set already declared");
					}
					else
					{
						setBlock = new PropertySetRegion(null, attributes);
						lastBlock = setBlock;
					}
				}
				else if (this.la.kind == 1)
				{
					this.lexer.NextToken();
					this.Error("set or get expected");
				}
				else
				{
					base.SynErr(164);
				}
				base.Expect(11);
				if (lastBlock != null)
				{
					lastBlock.StartLocation = startLocation;
					lastBlock.EndLocation = this.t.EndLocation;
				}
			}
		}

		private void GetAccessorDecl(out PropertyGetRegion getBlock, List<AttributeSection> attributes)
		{
			Statement stmt = null;
			base.Expect(1);
			if (this.t.val != "get")
			{
				this.Error("get expected");
			}
			Location startLocation = this.t.Location;
			if (this.la.kind == 16)
			{
				this.Block(out stmt);
			}
			else if (this.la.kind == 11)
			{
				this.lexer.NextToken();
			}
			else
			{
				base.SynErr(165);
			}
			getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes);
			getBlock.StartLocation = startLocation;
			getBlock.EndLocation = this.t.EndLocation;
		}

		private void SetAccessorDecl(out PropertySetRegion setBlock, List<AttributeSection> attributes)
		{
			Statement stmt = null;
			base.Expect(1);
			if (this.t.val != "set")
			{
				this.Error("set expected");
			}
			Location startLocation = this.t.Location;
			if (this.la.kind == 16)
			{
				this.Block(out stmt);
			}
			else if (this.la.kind == 11)
			{
				this.lexer.NextToken();
			}
			else
			{
				base.SynErr(166);
			}
			setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
			setBlock.StartLocation = startLocation;
			setBlock.EndLocation = this.t.EndLocation;
		}

		private void AddAccessorDecl(out Statement stmt)
		{
			stmt = null;
			base.Expect(1);
			if (this.t.val != "add")
			{
				this.Error("add expected");
			}
			this.Block(out stmt);
		}

		private void RemoveAccessorDecl(out Statement stmt)
		{
			stmt = null;
			base.Expect(1);
			if (this.t.val != "remove")
			{
				this.Error("remove expected");
			}
			this.Block(out stmt);
		}

		private void VariableInitializer(out Expression initializerExpression)
		{
			TypeReference type = null;
			Expression expr = null;
			initializerExpression = null;
			if (this.StartOf(5))
			{
				this.Expr(out initializerExpression);
			}
			else if (this.la.kind == 16)
			{
				this.ArrayInitializer(out initializerExpression);
			}
			else if (this.la.kind == 105)
			{
				this.lexer.NextToken();
				this.Type(out type);
				base.Expect(18);
				this.Expr(out expr);
				base.Expect(19);
				initializerExpression = new StackAllocExpression(type, expr);
			}
			else
			{
				base.SynErr(167);
			}
		}

		private void Statement()
		{
			Statement stmt = null;
			Location startPos = this.la.Location;
			if (this.IsLabel())
			{
				base.Expect(1);
				this.compilationUnit.AddChild(new LabelStatement(this.t.val));
				base.Expect(9);
				this.Statement();
			}
			else if (this.la.kind == 59)
			{
				this.lexer.NextToken();
				TypeReference type;
				this.Type(out type);
				LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifiers.Const);
				var.StartLocation = this.t.Location;
				base.Expect(1);
				string ident = this.t.val;
				base.Expect(3);
				Expression expr;
				this.Expr(out expr);
				var.Variables.Add(new VariableDeclaration(ident, expr));
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					base.Expect(1);
					ident = this.t.val;
					base.Expect(3);
					this.Expr(out expr);
					var.Variables.Add(new VariableDeclaration(ident, expr));
				}
				base.Expect(11);
				this.compilationUnit.AddChild(var);
			}
			else if (this.IsLocalVarDecl())
			{
				this.LocalVariableDecl(out stmt);
				base.Expect(11);
				this.compilationUnit.AddChild(stmt);
			}
			else if (this.StartOf(23))
			{
				this.EmbeddedStatement(out stmt);
				this.compilationUnit.AddChild(stmt);
			}
			else
			{
				base.SynErr(168);
			}
			if (stmt != null)
			{
				stmt.StartLocation = startPos;
				stmt.EndLocation = this.t.EndLocation;
			}
		}

		private void Argument(out Expression argumentexpr)
		{
			FieldDirection fd = FieldDirection.None;
			if (this.la.kind == 92 || this.la.kind == 99)
			{
				if (this.la.kind == 99)
				{
					this.lexer.NextToken();
					fd = FieldDirection.Ref;
				}
				else
				{
					this.lexer.NextToken();
					fd = FieldDirection.Out;
				}
			}
			Expression expr;
			this.Expr(out expr);
			Expression arg_81_1;
			if (fd == FieldDirection.None)
			{
				arg_81_1 = expr;
			}
			else
			{
				Expression expression;
				argumentexpr = (expression = new DirectionExpression(fd, expr));
				arg_81_1 = expression;
			}
			argumentexpr = arg_81_1;
		}

		private void ArrayInitializer(out Expression outExpr)
		{
			Expression expr = null;
			ArrayInitializerExpression initializer = new ArrayInitializerExpression();
			base.Expect(16);
			if (this.StartOf(24))
			{
				this.VariableInitializer(out expr);
				if (expr != null)
				{
					initializer.CreateExpressions.Add(expr);
				}
				while (this.NotFinalComma())
				{
					base.Expect(14);
					this.VariableInitializer(out expr);
					if (expr != null)
					{
						initializer.CreateExpressions.Add(expr);
					}
				}
				if (this.la.kind == 14)
				{
					this.lexer.NextToken();
				}
			}
			base.Expect(17);
			outExpr = initializer;
		}

		private void AssignmentOperator(out AssignmentOperatorType op)
		{
			op = AssignmentOperatorType.None;
			if (this.la.kind == 3)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.Assign;
			}
			else if (this.la.kind == 38)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.Add;
			}
			else if (this.la.kind == 39)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.Subtract;
			}
			else if (this.la.kind == 40)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.Multiply;
			}
			else if (this.la.kind == 41)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.Divide;
			}
			else if (this.la.kind == 42)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.Modulus;
			}
			else if (this.la.kind == 43)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.BitwiseAnd;
			}
			else if (this.la.kind == 44)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.BitwiseOr;
			}
			else if (this.la.kind == 45)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.ExclusiveOr;
			}
			else if (this.la.kind == 46)
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.ShiftLeft;
			}
			else if (this.la.kind == 22 && this.Peek(1).kind == 35)
			{
				base.Expect(22);
				base.Expect(35);
				op = AssignmentOperatorType.ShiftRight;
			}
			else
			{
				base.SynErr(169);
			}
		}

		private void LocalVariableDecl(out Statement stmt)
		{
			VariableDeclaration var = null;
			TypeReference type;
			this.Type(out type);
			LocalVariableDeclaration localVariableDeclaration = new LocalVariableDeclaration(type);
			localVariableDeclaration.StartLocation = this.t.Location;
			this.LocalVariableDeclarator(out var);
			localVariableDeclaration.Variables.Add(var);
			while (this.la.kind == 14)
			{
				this.lexer.NextToken();
				this.LocalVariableDeclarator(out var);
				localVariableDeclaration.Variables.Add(var);
			}
			stmt = localVariableDeclaration;
		}

		private void LocalVariableDeclarator(out VariableDeclaration var)
		{
			Expression expr = null;
			base.Expect(1);
			var = new VariableDeclaration(this.t.val);
			if (this.la.kind == 3)
			{
				this.lexer.NextToken();
				this.VariableInitializer(out expr);
				var.Initializer = expr;
			}
		}

		private void EmbeddedStatement(out Statement statement)
		{
			TypeReference type = null;
			Expression expr = null;
			Statement embeddedStatement = null;
			statement = null;
			if (this.la.kind == 16)
			{
				this.Block(out statement);
			}
			else if (this.la.kind == 11)
			{
				this.lexer.NextToken();
				statement = new EmptyStatement();
			}
			else if (this.UnCheckedAndLBrace())
			{
				bool isChecked = true;
				if (this.la.kind == 57)
				{
					this.lexer.NextToken();
				}
				else if (this.la.kind == 117)
				{
					this.lexer.NextToken();
					isChecked = false;
				}
				else
				{
					base.SynErr(170);
				}
				Statement block;
				this.Block(out block);
				statement = (isChecked ? new CheckedStatement(block) : new UncheckedStatement(block));
			}
			else if (this.la.kind == 78)
			{
				this.lexer.NextToken();
				Statement elseStatement = null;
				base.Expect(20);
				this.Expr(out expr);
				base.Expect(21);
				this.EmbeddedStatement(out embeddedStatement);
				if (this.la.kind == 66)
				{
					this.lexer.NextToken();
					this.EmbeddedStatement(out elseStatement);
				}
				statement = ((elseStatement != null) ? new IfElseStatement(expr, embeddedStatement, elseStatement) : new IfElseStatement(expr, embeddedStatement));
				if (elseStatement is IfElseStatement && (elseStatement as IfElseStatement).TrueStatement.Count == 1)
				{
					(statement as IfElseStatement).ElseIfSections.Add(new ElseIfSection((elseStatement as IfElseStatement).Condition, (elseStatement as IfElseStatement).TrueStatement[0]));
					(statement as IfElseStatement).ElseIfSections.AddRange((elseStatement as IfElseStatement).ElseIfSections);
					(statement as IfElseStatement).FalseStatement = (elseStatement as IfElseStatement).FalseStatement;
				}
			}
			else if (this.la.kind == 109)
			{
				this.lexer.NextToken();
				List<SwitchSection> switchSections = new List<SwitchSection>();
				base.Expect(20);
				this.Expr(out expr);
				base.Expect(21);
				base.Expect(16);
				this.SwitchSections(switchSections);
				base.Expect(17);
				statement = new SwitchStatement(expr, switchSections);
			}
			else if (this.la.kind == 124)
			{
				this.lexer.NextToken();
				base.Expect(20);
				this.Expr(out expr);
				base.Expect(21);
				this.EmbeddedStatement(out embeddedStatement);
				statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
			}
			else if (this.la.kind == 64)
			{
				this.lexer.NextToken();
				this.EmbeddedStatement(out embeddedStatement);
				base.Expect(124);
				base.Expect(20);
				this.Expr(out expr);
				base.Expect(21);
				base.Expect(11);
				statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End);
			}
			else if (this.la.kind == 75)
			{
				this.lexer.NextToken();
				List<Statement> initializer = null;
				List<Statement> iterator = null;
				base.Expect(20);
				if (this.StartOf(5))
				{
					this.ForInitializer(out initializer);
				}
				base.Expect(11);
				if (this.StartOf(5))
				{
					this.Expr(out expr);
				}
				base.Expect(11);
				if (this.StartOf(5))
				{
					this.ForIterator(out iterator);
				}
				base.Expect(21);
				this.EmbeddedStatement(out embeddedStatement);
				statement = new ForStatement(initializer, expr, iterator, embeddedStatement);
			}
			else if (this.la.kind == 76)
			{
				this.lexer.NextToken();
				base.Expect(20);
				this.Type(out type);
				base.Expect(1);
				string varName = this.t.val;
				Location start = this.t.Location;
				base.Expect(80);
				this.Expr(out expr);
				base.Expect(21);
				this.EmbeddedStatement(out embeddedStatement);
				statement = new ForeachStatement(type, varName, expr, embeddedStatement);
				statement.EndLocation = this.t.EndLocation;
			}
			else if (this.la.kind == 52)
			{
				this.lexer.NextToken();
				base.Expect(11);
				statement = new BreakStatement();
			}
			else if (this.la.kind == 60)
			{
				this.lexer.NextToken();
				base.Expect(11);
				statement = new ContinueStatement();
			}
			else if (this.la.kind == 77)
			{
				this.GotoStatement(out statement);
			}
			else if (this.IsYieldStatement())
			{
				base.Expect(1);
				if (this.la.kind == 100)
				{
					this.lexer.NextToken();
					this.Expr(out expr);
					statement = new YieldStatement(new ReturnStatement(expr));
				}
				else if (this.la.kind == 52)
				{
					this.lexer.NextToken();
					statement = new YieldStatement(new BreakStatement());
				}
				else
				{
					base.SynErr(171);
				}
				base.Expect(11);
			}
			else if (this.la.kind == 100)
			{
				this.lexer.NextToken();
				if (this.StartOf(5))
				{
					this.Expr(out expr);
				}
				base.Expect(11);
				statement = new ReturnStatement(expr);
			}
			else if (this.la.kind == 111)
			{
				this.lexer.NextToken();
				if (this.StartOf(5))
				{
					this.Expr(out expr);
				}
				base.Expect(11);
				statement = new ThrowStatement(expr);
			}
			else if (this.StartOf(5))
			{
				this.StatementExpr(out statement);
				base.Expect(11);
			}
			else if (this.la.kind == 113)
			{
				this.TryStatement(out statement);
			}
			else if (this.la.kind == 85)
			{
				this.lexer.NextToken();
				base.Expect(20);
				this.Expr(out expr);
				base.Expect(21);
				this.EmbeddedStatement(out embeddedStatement);
				statement = new LockStatement(expr, embeddedStatement);
			}
			else if (this.la.kind == 120)
			{
				Statement resourceAcquisitionStmt = null;
				this.lexer.NextToken();
				base.Expect(20);
				this.ResourceAcquisition(out resourceAcquisitionStmt);
				base.Expect(21);
				this.EmbeddedStatement(out embeddedStatement);
				statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement);
			}
			else if (this.la.kind == 118)
			{
				this.lexer.NextToken();
				this.Block(out embeddedStatement);
				statement = new UnsafeStatement(embeddedStatement);
			}
			else if (this.la.kind == 73)
			{
				this.lexer.NextToken();
				base.Expect(20);
				this.Type(out type);
				if (type.PointerNestingLevel == 0)
				{
					this.Error("can only fix pointer types");
				}
				List<VariableDeclaration> pointerDeclarators = new List<VariableDeclaration>(1);
				base.Expect(1);
				string identifier = this.t.val;
				base.Expect(3);
				this.Expr(out expr);
				pointerDeclarators.Add(new VariableDeclaration(identifier, expr));
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					base.Expect(1);
					identifier = this.t.val;
					base.Expect(3);
					this.Expr(out expr);
					pointerDeclarators.Add(new VariableDeclaration(identifier, expr));
				}
				base.Expect(21);
				this.EmbeddedStatement(out embeddedStatement);
				statement = new FixedStatement(type, pointerDeclarators, embeddedStatement);
			}
			else
			{
				base.SynErr(172);
			}
		}

		private void SwitchSections(List<SwitchSection> switchSections)
		{
			SwitchSection switchSection = new SwitchSection();
			CaseLabel label;
			this.SwitchLabel(out label);
			if (label != null)
			{
				switchSection.SwitchLabels.Add(label);
			}
			this.compilationUnit.BlockStart(switchSection);
			while (this.StartOf(25))
			{
				if (this.la.kind == 54 || this.la.kind == 62)
				{
					this.SwitchLabel(out label);
					if (label != null)
					{
						if (switchSection.Children.Count > 0)
						{
							this.compilationUnit.BlockEnd();
							switchSections.Add(switchSection);
							switchSection = new SwitchSection();
							this.compilationUnit.BlockStart(switchSection);
						}
						switchSection.SwitchLabels.Add(label);
					}
				}
				else
				{
					this.Statement();
				}
			}
			this.compilationUnit.BlockEnd();
			switchSections.Add(switchSection);
		}

		private void ForInitializer(out List<Statement> initializer)
		{
			initializer = new List<Statement>();
			if (this.IsLocalVarDecl())
			{
				Statement stmt;
				this.LocalVariableDecl(out stmt);
				initializer.Add(stmt);
			}
			else if (this.StartOf(5))
			{
				Statement stmt;
				this.StatementExpr(out stmt);
				initializer.Add(stmt);
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					this.StatementExpr(out stmt);
					initializer.Add(stmt);
				}
			}
			else
			{
				base.SynErr(173);
			}
		}

		private void ForIterator(out List<Statement> iterator)
		{
			iterator = new List<Statement>();
			Statement stmt;
			this.StatementExpr(out stmt);
			iterator.Add(stmt);
			while (this.la.kind == 14)
			{
				this.lexer.NextToken();
				this.StatementExpr(out stmt);
				iterator.Add(stmt);
			}
		}

		private void GotoStatement(out Statement stmt)
		{
			stmt = null;
			base.Expect(77);
			if (this.la.kind == 1)
			{
				this.lexer.NextToken();
				stmt = new GotoStatement(this.t.val);
				base.Expect(11);
			}
			else if (this.la.kind == 54)
			{
				this.lexer.NextToken();
				Expression expr;
				this.Expr(out expr);
				base.Expect(11);
				stmt = new GotoCaseStatement(expr);
			}
			else if (this.la.kind == 62)
			{
				this.lexer.NextToken();
				base.Expect(11);
				stmt = new GotoCaseStatement(null);
			}
			else
			{
				base.SynErr(174);
			}
		}

		private void StatementExpr(out Statement stmt)
		{
			Expression expr;
			this.Expr(out expr);
			stmt = new ExpressionStatement(expr);
		}

		private void TryStatement(out Statement tryStatement)
		{
			Statement blockStmt = null;
			Statement finallyStmt = null;
			List<CatchClause> catchClauses = null;
			base.Expect(113);
			this.Block(out blockStmt);
			if (this.la.kind == 55)
			{
				this.CatchClauses(out catchClauses);
				if (this.la.kind == 72)
				{
					this.lexer.NextToken();
					this.Block(out finallyStmt);
				}
			}
			else if (this.la.kind == 72)
			{
				this.lexer.NextToken();
				this.Block(out finallyStmt);
			}
			else
			{
				base.SynErr(175);
			}
			tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		}

		private void ResourceAcquisition(out Statement stmt)
		{
			stmt = null;
			if (this.IsLocalVarDecl())
			{
				this.LocalVariableDecl(out stmt);
			}
			else if (this.StartOf(5))
			{
				Expression expr;
				this.Expr(out expr);
				stmt = new ExpressionStatement(expr);
			}
			else
			{
				base.SynErr(176);
			}
		}

		private void SwitchLabel(out CaseLabel label)
		{
			Expression expr = null;
			label = null;
			if (this.la.kind == 54)
			{
				this.lexer.NextToken();
				this.Expr(out expr);
				base.Expect(9);
				label = new CaseLabel(expr);
			}
			else if (this.la.kind == 62)
			{
				this.lexer.NextToken();
				base.Expect(9);
				label = new CaseLabel();
			}
			else
			{
				base.SynErr(177);
			}
		}

		private void CatchClauses(out List<CatchClause> catchClauses)
		{
			catchClauses = new List<CatchClause>();
			base.Expect(55);
			if (this.la.kind == 16)
			{
				Statement stmt;
				this.Block(out stmt);
				catchClauses.Add(new CatchClause(stmt));
			}
			else if (this.la.kind == 20)
			{
				this.lexer.NextToken();
				TypeReference typeRef;
				this.ClassType(out typeRef, false);
				string identifier = null;
				if (this.la.kind == 1)
				{
					this.lexer.NextToken();
					identifier = this.t.val;
				}
				base.Expect(21);
				Statement stmt;
				this.Block(out stmt);
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt));
				while (this.IsTypedCatch())
				{
					base.Expect(55);
					base.Expect(20);
					this.ClassType(out typeRef, false);
					identifier = null;
					if (this.la.kind == 1)
					{
						this.lexer.NextToken();
						identifier = this.t.val;
					}
					base.Expect(21);
					this.Block(out stmt);
					catchClauses.Add(new CatchClause(typeRef, identifier, stmt));
				}
				if (this.la.kind == 55)
				{
					this.lexer.NextToken();
					this.Block(out stmt);
					catchClauses.Add(new CatchClause(stmt));
				}
			}
			else
			{
				base.SynErr(178);
			}
		}

		private void UnaryExpr(out Expression uExpr)
		{
			TypeReference type = null;
			Expression expr = null;
			ArrayList expressions = new ArrayList();
			uExpr = null;
			while (this.StartOf(26) || this.IsTypeCast())
			{
				if (this.la.kind == 4)
				{
					this.lexer.NextToken();
					expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus));
				}
				else if (this.la.kind == 5)
				{
					this.lexer.NextToken();
					expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus));
				}
				else if (this.la.kind == 24)
				{
					this.lexer.NextToken();
					expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not));
				}
				else if (this.la.kind == 27)
				{
					this.lexer.NextToken();
					expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot));
				}
				else if (this.la.kind == 6)
				{
					this.lexer.NextToken();
					expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Star));
				}
				else if (this.la.kind == 31)
				{
					this.lexer.NextToken();
					expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment));
				}
				else if (this.la.kind == 32)
				{
					this.lexer.NextToken();
					expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement));
				}
				else if (this.la.kind == 28)
				{
					this.lexer.NextToken();
					expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitWiseAnd));
				}
				else
				{
					base.Expect(20);
					this.Type(out type);
					base.Expect(21);
					expressions.Add(new CastExpression(type));
				}
			}
			if (this.LastExpressionIsUnaryMinus(expressions) && this.IsMostNegativeIntegerWithoutTypeSuffix())
			{
				base.Expect(2);
				expressions.RemoveAt(expressions.Count - 1);
				if (this.t.literalValue is uint)
				{
					expr = new PrimitiveExpression(-2147483648, -2147483648.ToString());
				}
				else
				{
					if (!(this.t.literalValue is ulong))
					{
						throw new Exception("t.literalValue must be uint or ulong");
					}
					expr = new PrimitiveExpression(-9223372036854775808L, -9223372036854775808L.ToString());
				}
			}
			else if (this.StartOf(27))
			{
				this.PrimaryExpr(out expr);
			}
			else
			{
				base.SynErr(179);
			}
			for (int i = 0; i < expressions.Count; i++)
			{
				Expression nextExpression = (i + 1 < expressions.Count) ? ((Expression)expressions[i + 1]) : expr;
				if (expressions[i] is CastExpression)
				{
					((CastExpression)expressions[i]).Expression = nextExpression;
				}
				else
				{
					((UnaryOperatorExpression)expressions[i]).Expression = nextExpression;
				}
			}
			if (expressions.Count > 0)
			{
				uExpr = (Expression)expressions[0];
			}
			else
			{
				uExpr = expr;
			}
		}

		private void ConditionalOrExpr(ref Expression outExpr)
		{
			this.ConditionalAndExpr(ref outExpr);
			while (this.la.kind == 26)
			{
				this.lexer.NextToken();
				Expression expr;
				this.UnaryExpr(out expr);
				this.ConditionalAndExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);
			}
		}

		private void PrimaryExpr(out Expression pexpr)
		{
			TypeReference type = null;
			List<TypeReference> typeList = null;
			bool isArrayCreation = false;
			pexpr = null;
			if (this.la.kind == 112)
			{
				this.lexer.NextToken();
				pexpr = new PrimitiveExpression(true, "true");
			}
			else if (this.la.kind == 71)
			{
				this.lexer.NextToken();
				pexpr = new PrimitiveExpression(false, "false");
			}
			else if (this.la.kind == 89)
			{
				this.lexer.NextToken();
				pexpr = new PrimitiveExpression(null, "null");
			}
			else if (this.la.kind == 2)
			{
				this.lexer.NextToken();
				pexpr = new PrimitiveExpression(this.t.literalValue, this.t.val);
			}
			else if (this.la.kind == 1 && this.Peek(1).kind == 10)
			{
				base.Expect(1);
				type = new TypeReference(this.t.val);
				base.Expect(10);
				pexpr = new TypeReferenceExpression(type);
				base.Expect(1);
				if (type.Type == "global")
				{
					type.IsGlobal = true;
					type.Type = (this.t.val ?? "?");
				}
				else
				{
					TypeReference expr_1A4 = type;
					expr_1A4.Type = expr_1A4.Type + "." + (this.t.val ?? "?");
				}
			}
			else if (this.la.kind == 1)
			{
				this.lexer.NextToken();
				pexpr = new IdentifierExpression(this.t.val);
			}
			else if (this.la.kind == 20)
			{
				this.lexer.NextToken();
				Expression expr;
				this.Expr(out expr);
				base.Expect(21);
				pexpr = new ParenthesizedExpression(expr);
			}
			else if (this.StartOf(28))
			{
				string val = null;
				int kind = this.la.kind;
				if (kind <= 74)
				{
					if (kind <= 56)
					{
						switch (kind)
						{
						case 51:
							this.lexer.NextToken();
							val = "bool";
							break;
						case 52:
							break;
						case 53:
							this.lexer.NextToken();
							val = "byte";
							break;
						default:
							if (kind == 56)
							{
								this.lexer.NextToken();
								val = "char";
							}
							break;
						}
					}
					else if (kind != 61)
					{
						if (kind != 65)
						{
							if (kind == 74)
							{
								this.lexer.NextToken();
								val = "float";
							}
						}
						else
						{
							this.lexer.NextToken();
							val = "double";
						}
					}
					else
					{
						this.lexer.NextToken();
						val = "decimal";
					}
				}
				else if (kind <= 90)
				{
					if (kind != 81)
					{
						if (kind != 86)
						{
							if (kind == 90)
							{
								this.lexer.NextToken();
								val = "object";
							}
						}
						else
						{
							this.lexer.NextToken();
							val = "long";
						}
					}
					else
					{
						this.lexer.NextToken();
						val = "int";
					}
				}
				else
				{
					switch (kind)
					{
					case 101:
						this.lexer.NextToken();
						val = "sbyte";
						break;
					case 102:
						break;
					case 103:
						this.lexer.NextToken();
						val = "short";
						break;
					default:
						if (kind != 107)
						{
							switch (kind)
							{
							case 115:
								this.lexer.NextToken();
								val = "uint";
								break;
							case 116:
								this.lexer.NextToken();
								val = "ulong";
								break;
							case 119:
								this.lexer.NextToken();
								val = "ushort";
								break;
							}
						}
						else
						{
							this.lexer.NextToken();
							val = "string";
						}
						break;
					}
				}
				this.t.val = "";
				base.Expect(15);
				base.Expect(1);
				pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), this.t.val);
			}
			else if (this.la.kind == 110)
			{
				this.lexer.NextToken();
				pexpr = new ThisReferenceExpression();
			}
			else if (this.la.kind == 50)
			{
				this.lexer.NextToken();
				Expression retExpr = new BaseReferenceExpression();
				if (this.la.kind == 15)
				{
					this.lexer.NextToken();
					base.Expect(1);
					retExpr = new FieldReferenceExpression(retExpr, this.t.val);
				}
				else if (this.la.kind == 18)
				{
					this.lexer.NextToken();
					Expression expr;
					this.Expr(out expr);
					List<Expression> indices = new List<Expression>();
					if (expr != null)
					{
						indices.Add(expr);
					}
					while (this.la.kind == 14)
					{
						this.lexer.NextToken();
						this.Expr(out expr);
						if (expr != null)
						{
							indices.Add(expr);
						}
					}
					base.Expect(19);
					retExpr = new IndexerExpression(retExpr, indices);
				}
				else
				{
					base.SynErr(180);
				}
				pexpr = retExpr;
			}
			else if (this.la.kind == 88)
			{
				this.lexer.NextToken();
				this.NonArrayType(out type);
				List<Expression> parameters = new List<Expression>();
				if (this.la.kind == 20)
				{
					this.lexer.NextToken();
					ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters);
					if (this.StartOf(21))
					{
						Expression expr;
						this.Argument(out expr);
						if (expr != null)
						{
							parameters.Add(expr);
						}
						while (this.la.kind == 14)
						{
							this.lexer.NextToken();
							this.Argument(out expr);
							if (expr != null)
							{
								parameters.Add(expr);
							}
						}
					}
					base.Expect(21);
					pexpr = oce;
				}
				else if (this.la.kind == 18)
				{
					this.lexer.NextToken();
					isArrayCreation = true;
					ArrayCreateExpression ace = new ArrayCreateExpression(type);
					pexpr = ace;
					int dims = 0;
					List<int> ranks = new List<int>();
					if (this.la.kind == 14 || this.la.kind == 19)
					{
						while (this.la.kind == 14)
						{
							this.lexer.NextToken();
							dims++;
						}
						base.Expect(19);
						ranks.Add(dims);
						dims = 0;
						while (this.la.kind == 18)
						{
							this.lexer.NextToken();
							while (this.la.kind == 14)
							{
								this.lexer.NextToken();
								dims++;
							}
							base.Expect(19);
							ranks.Add(dims);
							dims = 0;
						}
						ace.CreateType.RankSpecifier = ranks.ToArray();
						Expression expr;
						this.ArrayInitializer(out expr);
						ace.ArrayInitializer = (ArrayInitializerExpression)expr;
					}
					else if (this.StartOf(5))
					{
						Expression expr;
						this.Expr(out expr);
						if (expr != null)
						{
							parameters.Add(expr);
						}
						while (this.la.kind == 14)
						{
							this.lexer.NextToken();
							dims++;
							this.Expr(out expr);
							if (expr != null)
							{
								parameters.Add(expr);
							}
						}
						base.Expect(19);
						ranks.Add(dims);
						ace.Arguments = parameters;
						dims = 0;
						while (this.la.kind == 18)
						{
							this.lexer.NextToken();
							while (this.la.kind == 14)
							{
								this.lexer.NextToken();
								dims++;
							}
							base.Expect(19);
							ranks.Add(dims);
							dims = 0;
						}
						ace.CreateType.RankSpecifier = ranks.ToArray();
						if (this.la.kind == 16)
						{
							this.ArrayInitializer(out expr);
							ace.ArrayInitializer = (ArrayInitializerExpression)expr;
						}
					}
					else
					{
						base.SynErr(181);
					}
				}
				else
				{
					base.SynErr(182);
				}
			}
			else if (this.la.kind == 114)
			{
				this.lexer.NextToken();
				base.Expect(20);
				if (this.NotVoidPointer())
				{
					base.Expect(122);
					type = new TypeReference("void");
				}
				else if (this.StartOf(9))
				{
					this.TypeWithRestriction(out type, true, true);
				}
				else
				{
					base.SynErr(183);
				}
				base.Expect(21);
				pexpr = new TypeOfExpression(type);
			}
			else if (this.la.kind == 62)
			{
				this.lexer.NextToken();
				base.Expect(20);
				this.Type(out type);
				base.Expect(21);
				pexpr = new DefaultValueExpression(type);
			}
			else if (this.la.kind == 104)
			{
				this.lexer.NextToken();
				base.Expect(20);
				this.Type(out type);
				base.Expect(21);
				pexpr = new SizeOfExpression(type);
			}
			else if (this.la.kind == 57)
			{
				this.lexer.NextToken();
				base.Expect(20);
				Expression expr;
				this.Expr(out expr);
				base.Expect(21);
				pexpr = new CheckedExpression(expr);
			}
			else if (this.la.kind == 117)
			{
				this.lexer.NextToken();
				base.Expect(20);
				Expression expr;
				this.Expr(out expr);
				base.Expect(21);
				pexpr = new UncheckedExpression(expr);
			}
			else if (this.la.kind == 63)
			{
				this.lexer.NextToken();
				Expression expr;
				this.AnonymousMethodExpr(out expr);
				pexpr = expr;
			}
			else
			{
				base.SynErr(184);
			}
			while (this.StartOf(29) || (this.IsGenericFollowedBy(15) && this.IsTypeReferenceExpression(pexpr)) || this.IsGenericFollowedBy(20))
			{
				if (this.la.kind == 31 || this.la.kind == 32)
				{
					if (this.la.kind == 31)
					{
						this.lexer.NextToken();
						pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement);
					}
					else if (this.la.kind == 32)
					{
						this.lexer.NextToken();
						pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement);
					}
					else
					{
						base.SynErr(185);
					}
				}
				else if (this.la.kind == 47)
				{
					this.lexer.NextToken();
					base.Expect(1);
					pexpr = new PointerReferenceExpression(pexpr, this.t.val);
				}
				else if (this.la.kind == 15)
				{
					this.lexer.NextToken();
					base.Expect(1);
					pexpr = new FieldReferenceExpression(pexpr, this.t.val);
				}
				else if (this.IsGenericFollowedBy(15) && this.IsTypeReferenceExpression(pexpr))
				{
					this.TypeArgumentList(out typeList, false);
					base.Expect(15);
					base.Expect(1);
					pexpr = new FieldReferenceExpression(this.GetTypeReferenceExpression(pexpr, typeList), this.t.val);
				}
				else if (this.la.kind == 20)
				{
					this.lexer.NextToken();
					List<Expression> parameters = new List<Expression>();
					if (this.StartOf(21))
					{
						Expression expr;
						this.Argument(out expr);
						if (expr != null)
						{
							parameters.Add(expr);
						}
						while (this.la.kind == 14)
						{
							this.lexer.NextToken();
							this.Argument(out expr);
							if (expr != null)
							{
								parameters.Add(expr);
							}
						}
					}
					base.Expect(21);
					pexpr = new InvocationExpression(pexpr, parameters);
				}
				else if (this.IsGenericFollowedBy(20))
				{
					this.TypeArgumentList(out typeList, false);
					base.Expect(20);
					List<Expression> parameters = new List<Expression>();
					if (this.StartOf(21))
					{
						Expression expr;
						this.Argument(out expr);
						if (expr != null)
						{
							parameters.Add(expr);
						}
						while (this.la.kind == 14)
						{
							this.lexer.NextToken();
							this.Argument(out expr);
							if (expr != null)
							{
								parameters.Add(expr);
							}
						}
					}
					base.Expect(21);
					pexpr = new InvocationExpression(pexpr, parameters, typeList);
				}
				else
				{
					if (isArrayCreation)
					{
						this.Error("element access not allow on array creation");
					}
					List<Expression> indices = new List<Expression>();
					this.lexer.NextToken();
					Expression expr;
					this.Expr(out expr);
					if (expr != null)
					{
						indices.Add(expr);
					}
					while (this.la.kind == 14)
					{
						this.lexer.NextToken();
						this.Expr(out expr);
						if (expr != null)
						{
							indices.Add(expr);
						}
					}
					base.Expect(19);
					pexpr = new IndexerExpression(pexpr, indices);
				}
			}
		}

		private void AnonymousMethodExpr(out Expression outExpr)
		{
			AnonymousMethodExpression expr = new AnonymousMethodExpression();
			expr.StartLocation = this.t.Location;
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			outExpr = expr;
			if (this.la.kind == 20)
			{
				this.lexer.NextToken();
				if (this.StartOf(10))
				{
					this.FormalParameterList(p);
					expr.Parameters = p;
				}
				base.Expect(21);
				expr.HasParameterList = true;
			}
			if (this.compilationUnit != null)
			{
				Statement stmt;
				this.Block(out stmt);
				expr.Body = (BlockStatement)stmt;
			}
			else
			{
				base.Expect(16);
				this.lexer.SkipCurrentBlock(0);
				base.Expect(17);
			}
			expr.EndLocation = this.t.Location;
		}

		private void TypeArgumentList(out List<TypeReference> types, bool canBeUnbound)
		{
			types = new List<TypeReference>();
			TypeReference type = null;
			base.Expect(23);
			if (canBeUnbound && (this.la.kind == 22 || this.la.kind == 14))
			{
				types.Add(TypeReference.Null);
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					types.Add(TypeReference.Null);
				}
			}
			else if (this.StartOf(9))
			{
				this.Type(out type);
				if (type != null)
				{
					types.Add(type);
				}
				while (this.la.kind == 14)
				{
					this.lexer.NextToken();
					this.Type(out type);
					if (type != null)
					{
						types.Add(type);
					}
				}
			}
			else
			{
				base.SynErr(186);
			}
			base.Expect(22);
		}

		private void ConditionalAndExpr(ref Expression outExpr)
		{
			this.InclusiveOrExpr(ref outExpr);
			while (this.la.kind == 25)
			{
				this.lexer.NextToken();
				Expression expr;
				this.UnaryExpr(out expr);
				this.InclusiveOrExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);
			}
		}

		private void InclusiveOrExpr(ref Expression outExpr)
		{
			this.ExclusiveOrExpr(ref outExpr);
			while (this.la.kind == 29)
			{
				this.lexer.NextToken();
				Expression expr;
				this.UnaryExpr(out expr);
				this.ExclusiveOrExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);
			}
		}

		private void ExclusiveOrExpr(ref Expression outExpr)
		{
			this.AndExpr(ref outExpr);
			while (this.la.kind == 30)
			{
				this.lexer.NextToken();
				Expression expr;
				this.UnaryExpr(out expr);
				this.AndExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);
			}
		}

		private void AndExpr(ref Expression outExpr)
		{
			this.EqualityExpr(ref outExpr);
			while (this.la.kind == 28)
			{
				this.lexer.NextToken();
				Expression expr;
				this.UnaryExpr(out expr);
				this.EqualityExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);
			}
		}

		private void EqualityExpr(ref Expression outExpr)
		{
			this.RelationalExpr(ref outExpr);
			while (this.la.kind == 33 || this.la.kind == 34)
			{
				BinaryOperatorType op;
				if (this.la.kind == 34)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.InEquality;
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Equality;
				}
				Expression expr;
				this.UnaryExpr(out expr);
				this.RelationalExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void RelationalExpr(ref Expression outExpr)
		{
			BinaryOperatorType op = BinaryOperatorType.None;
			this.ShiftExpr(ref outExpr);
			while (this.StartOf(30))
			{
				if (this.StartOf(31))
				{
					if (this.la.kind == 23)
					{
						this.lexer.NextToken();
						op = BinaryOperatorType.LessThan;
					}
					else if (this.la.kind == 22)
					{
						this.lexer.NextToken();
						op = BinaryOperatorType.GreaterThan;
					}
					else if (this.la.kind == 36)
					{
						this.lexer.NextToken();
						op = BinaryOperatorType.LessThanOrEqual;
					}
					else if (this.la.kind == 35)
					{
						this.lexer.NextToken();
						op = BinaryOperatorType.GreaterThanOrEqual;
					}
					else
					{
						base.SynErr(187);
					}
					Expression expr;
					this.UnaryExpr(out expr);
					this.ShiftExpr(ref expr);
					outExpr = new BinaryOperatorExpression(outExpr, op, expr);
				}
				else if (this.la.kind == 84)
				{
					this.lexer.NextToken();
					TypeReference type;
					this.TypeWithRestriction(out type, false, false);
					if (this.la.kind == 12 && !Tokens.CastFollower[this.Peek(1).kind])
					{
						this.NullableQuestionMark(ref type);
					}
					outExpr = new TypeOfIsExpression(outExpr, type);
				}
				else if (this.la.kind == 49)
				{
					this.lexer.NextToken();
					TypeReference type;
					this.TypeWithRestriction(out type, false, false);
					if (this.la.kind == 12 && !Tokens.CastFollower[this.Peek(1).kind])
					{
						this.NullableQuestionMark(ref type);
					}
					outExpr = new CastExpression(type, outExpr, CastType.TryCast);
				}
				else
				{
					base.SynErr(188);
				}
			}
		}

		private void ShiftExpr(ref Expression outExpr)
		{
			this.AdditiveExpr(ref outExpr);
			while (this.la.kind == 37 || this.IsShiftRight())
			{
				BinaryOperatorType op;
				if (this.la.kind == 37)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.ShiftLeft;
				}
				else
				{
					base.Expect(22);
					base.Expect(22);
					op = BinaryOperatorType.ShiftRight;
				}
				Expression expr;
				this.UnaryExpr(out expr);
				this.AdditiveExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void AdditiveExpr(ref Expression outExpr)
		{
			this.MultiplicativeExpr(ref outExpr);
			while (this.la.kind == 4 || this.la.kind == 5)
			{
				BinaryOperatorType op;
				if (this.la.kind == 4)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Add;
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Subtract;
				}
				Expression expr;
				this.UnaryExpr(out expr);
				this.MultiplicativeExpr(ref expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void MultiplicativeExpr(ref Expression outExpr)
		{
			while (this.la.kind == 6 || this.la.kind == 7 || this.la.kind == 8)
			{
				BinaryOperatorType op;
				if (this.la.kind == 6)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Multiply;
				}
				else if (this.la.kind == 7)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Divide;
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Modulus;
				}
				Expression expr;
				this.UnaryExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void TypeParameterConstraintsClauseBase(out TypeReference type)
		{
			type = null;
			if (this.la.kind == 108)
			{
				this.lexer.NextToken();
				type = TypeReference.StructConstraint;
			}
			else if (this.la.kind == 58)
			{
				this.lexer.NextToken();
				type = TypeReference.ClassConstraint;
			}
			else if (this.la.kind == 88)
			{
				this.lexer.NextToken();
				base.Expect(20);
				base.Expect(21);
				type = TypeReference.NewConstraint;
			}
			else if (this.StartOf(9))
			{
				TypeReference t;
				this.Type(out t);
				type = t;
			}
			else
			{
				base.SynErr(189);
			}
		}

		public override void Parse()
		{
			this.CS();
		}

		protected override void SynErr(int line, int col, int errorNumber)
		{
			string s;
			switch (errorNumber)
			{
			case 0:
				s = "EOF expected";
				break;
			case 1:
				s = "ident expected";
				break;
			case 2:
				s = "Literal expected";
				break;
			case 3:
				s = "\"=\" expected";
				break;
			case 4:
				s = "\"+\" expected";
				break;
			case 5:
				s = "\"-\" expected";
				break;
			case 6:
				s = "\"*\" expected";
				break;
			case 7:
				s = "\"/\" expected";
				break;
			case 8:
				s = "\"%\" expected";
				break;
			case 9:
				s = "\":\" expected";
				break;
			case 10:
				s = "\"::\" expected";
				break;
			case 11:
				s = "\";\" expected";
				break;
			case 12:
				s = "\"?\" expected";
				break;
			case 13:
				s = "\"??\" expected";
				break;
			case 14:
				s = "\",\" expected";
				break;
			case 15:
				s = "\".\" expected";
				break;
			case 16:
				s = "\"{\" expected";
				break;
			case 17:
				s = "\"}\" expected";
				break;
			case 18:
				s = "\"[\" expected";
				break;
			case 19:
				s = "\"]\" expected";
				break;
			case 20:
				s = "\"(\" expected";
				break;
			case 21:
				s = "\")\" expected";
				break;
			case 22:
				s = "\">\" expected";
				break;
			case 23:
				s = "\"<\" expected";
				break;
			case 24:
				s = "\"!\" expected";
				break;
			case 25:
				s = "\"&&\" expected";
				break;
			case 26:
				s = "\"||\" expected";
				break;
			case 27:
				s = "\"~\" expected";
				break;
			case 28:
				s = "\"&\" expected";
				break;
			case 29:
				s = "\"|\" expected";
				break;
			case 30:
				s = "\"^\" expected";
				break;
			case 31:
				s = "\"++\" expected";
				break;
			case 32:
				s = "\"--\" expected";
				break;
			case 33:
				s = "\"==\" expected";
				break;
			case 34:
				s = "\"!=\" expected";
				break;
			case 35:
				s = "\">=\" expected";
				break;
			case 36:
				s = "\"<=\" expected";
				break;
			case 37:
				s = "\"<<\" expected";
				break;
			case 38:
				s = "\"+=\" expected";
				break;
			case 39:
				s = "\"-=\" expected";
				break;
			case 40:
				s = "\"*=\" expected";
				break;
			case 41:
				s = "\"/=\" expected";
				break;
			case 42:
				s = "\"%=\" expected";
				break;
			case 43:
				s = "\"&=\" expected";
				break;
			case 44:
				s = "\"|=\" expected";
				break;
			case 45:
				s = "\"^=\" expected";
				break;
			case 46:
				s = "\"<<=\" expected";
				break;
			case 47:
				s = "\"->\" expected";
				break;
			case 48:
				s = "\"abstract\" expected";
				break;
			case 49:
				s = "\"as\" expected";
				break;
			case 50:
				s = "\"base\" expected";
				break;
			case 51:
				s = "\"bool\" expected";
				break;
			case 52:
				s = "\"break\" expected";
				break;
			case 53:
				s = "\"byte\" expected";
				break;
			case 54:
				s = "\"case\" expected";
				break;
			case 55:
				s = "\"catch\" expected";
				break;
			case 56:
				s = "\"char\" expected";
				break;
			case 57:
				s = "\"checked\" expected";
				break;
			case 58:
				s = "\"class\" expected";
				break;
			case 59:
				s = "\"const\" expected";
				break;
			case 60:
				s = "\"continue\" expected";
				break;
			case 61:
				s = "\"decimal\" expected";
				break;
			case 62:
				s = "\"default\" expected";
				break;
			case 63:
				s = "\"delegate\" expected";
				break;
			case 64:
				s = "\"do\" expected";
				break;
			case 65:
				s = "\"double\" expected";
				break;
			case 66:
				s = "\"else\" expected";
				break;
			case 67:
				s = "\"enum\" expected";
				break;
			case 68:
				s = "\"event\" expected";
				break;
			case 69:
				s = "\"explicit\" expected";
				break;
			case 70:
				s = "\"extern\" expected";
				break;
			case 71:
				s = "\"false\" expected";
				break;
			case 72:
				s = "\"finally\" expected";
				break;
			case 73:
				s = "\"fixed\" expected";
				break;
			case 74:
				s = "\"float\" expected";
				break;
			case 75:
				s = "\"for\" expected";
				break;
			case 76:
				s = "\"foreach\" expected";
				break;
			case 77:
				s = "\"goto\" expected";
				break;
			case 78:
				s = "\"if\" expected";
				break;
			case 79:
				s = "\"implicit\" expected";
				break;
			case 80:
				s = "\"in\" expected";
				break;
			case 81:
				s = "\"int\" expected";
				break;
			case 82:
				s = "\"interface\" expected";
				break;
			case 83:
				s = "\"internal\" expected";
				break;
			case 84:
				s = "\"is\" expected";
				break;
			case 85:
				s = "\"lock\" expected";
				break;
			case 86:
				s = "\"long\" expected";
				break;
			case 87:
				s = "\"namespace\" expected";
				break;
			case 88:
				s = "\"new\" expected";
				break;
			case 89:
				s = "\"null\" expected";
				break;
			case 90:
				s = "\"object\" expected";
				break;
			case 91:
				s = "\"operator\" expected";
				break;
			case 92:
				s = "\"out\" expected";
				break;
			case 93:
				s = "\"override\" expected";
				break;
			case 94:
				s = "\"params\" expected";
				break;
			case 95:
				s = "\"private\" expected";
				break;
			case 96:
				s = "\"protected\" expected";
				break;
			case 97:
				s = "\"public\" expected";
				break;
			case 98:
				s = "\"readonly\" expected";
				break;
			case 99:
				s = "\"ref\" expected";
				break;
			case 100:
				s = "\"return\" expected";
				break;
			case 101:
				s = "\"sbyte\" expected";
				break;
			case 102:
				s = "\"sealed\" expected";
				break;
			case 103:
				s = "\"short\" expected";
				break;
			case 104:
				s = "\"sizeof\" expected";
				break;
			case 105:
				s = "\"stackalloc\" expected";
				break;
			case 106:
				s = "\"static\" expected";
				break;
			case 107:
				s = "\"string\" expected";
				break;
			case 108:
				s = "\"struct\" expected";
				break;
			case 109:
				s = "\"switch\" expected";
				break;
			case 110:
				s = "\"this\" expected";
				break;
			case 111:
				s = "\"throw\" expected";
				break;
			case 112:
				s = "\"true\" expected";
				break;
			case 113:
				s = "\"try\" expected";
				break;
			case 114:
				s = "\"typeof\" expected";
				break;
			case 115:
				s = "\"uint\" expected";
				break;
			case 116:
				s = "\"ulong\" expected";
				break;
			case 117:
				s = "\"unchecked\" expected";
				break;
			case 118:
				s = "\"unsafe\" expected";
				break;
			case 119:
				s = "\"ushort\" expected";
				break;
			case 120:
				s = "\"using\" expected";
				break;
			case 121:
				s = "\"virtual\" expected";
				break;
			case 122:
				s = "\"void\" expected";
				break;
			case 123:
				s = "\"volatile\" expected";
				break;
			case 124:
				s = "\"while\" expected";
				break;
			case 125:
				s = "??? expected";
				break;
			case 126:
				s = "invalid Namespace Member Declaration";
				break;
			case 127:
				s = "invalid Non Array Type";
				break;
			case 128:
				s = "invalid Attribute Arguments";
				break;
			case 129:
				s = "invalid Expression";
				break;
			case 130:
				s = "invalid Type Modifier";
				break;
			case 131:
				s = "invalid Type Declaration";
				break;
			case 132:
				s = "invalid Type Declaration";
				break;
			case 133:
				s = "invalid Integral Type";
				break;
			case 134:
				s = "invalid Formal Parameter List";
				break;
			case 135:
				s = "invalid Formal Parameter List";
				break;
			case 136:
				s = "invalid Class Type";
				break;
			case 137:
				s = "invalid Class Member Declaration";
				break;
			case 138:
				s = "invalid Class Member Declaration";
				break;
			case 139:
				s = "invalid Struct Member Declaration";
				break;
			case 140:
				s = "invalid Struct Member Declaration";
				break;
			case 141:
				s = "invalid Struct Member Declaration";
				break;
			case 142:
				s = "invalid Struct Member Declaration";
				break;
			case 143:
				s = "invalid Struct Member Declaration";
				break;
			case 144:
				s = "invalid Struct Member Declaration";
				break;
			case 145:
				s = "invalid Struct Member Declaration";
				break;
			case 146:
				s = "invalid Struct Member Declaration";
				break;
			case 147:
				s = "invalid Struct Member Declaration";
				break;
			case 148:
				s = "invalid Struct Member Declaration";
				break;
			case 149:
				s = "invalid Struct Member Declaration";
				break;
			case 150:
				s = "invalid Struct Member Declaration";
				break;
			case 151:
				s = "invalid Struct Member Declaration";
				break;
			case 152:
				s = "invalid Interface Member Declaration";
				break;
			case 153:
				s = "invalid Interface Member Declaration";
				break;
			case 154:
				s = "invalid Interface Member Declaration";
				break;
			case 155:
				s = "invalid Type With Restriction";
				break;
			case 156:
				s = "invalid Type With Restriction";
				break;
			case 157:
				s = "invalid Simple Type";
				break;
			case 158:
				s = "invalid Accessor Modifiers";
				break;
			case 159:
				s = "invalid Event Accessor Declaration";
				break;
			case 160:
				s = "invalid Constructor Initializer";
				break;
			case 161:
				s = "invalid Overloadable Operator";
				break;
			case 162:
				s = "invalid Accessor Decls";
				break;
			case 163:
				s = "invalid Interface Accessors";
				break;
			case 164:
				s = "invalid Interface Accessors";
				break;
			case 165:
				s = "invalid Get Accessor Declaration";
				break;
			case 166:
				s = "invalid Set Accessor Declaration";
				break;
			case 167:
				s = "invalid Variable Initializer";
				break;
			case 168:
				s = "invalid Statement";
				break;
			case 169:
				s = "invalid Assignment Operator";
				break;
			case 170:
				s = "invalid Embedded Statement";
				break;
			case 171:
				s = "invalid Embedded Statement";
				break;
			case 172:
				s = "invalid Embedded Statement";
				break;
			case 173:
				s = "invalid For Initializer";
				break;
			case 174:
				s = "invalid Goto Statement";
				break;
			case 175:
				s = "invalid Try Statement";
				break;
			case 176:
				s = "invalid Resource Acquisition";
				break;
			case 177:
				s = "invalid Switch Label";
				break;
			case 178:
				s = "invalid Catch Clauses";
				break;
			case 179:
				s = "invalid Unary Expression";
				break;
			case 180:
				s = "invalid Primary Expression";
				break;
			case 181:
				s = "invalid Primary Expression";
				break;
			case 182:
				s = "invalid Primary Expression";
				break;
			case 183:
				s = "invalid Primary Expression";
				break;
			case 184:
				s = "invalid Primary Expression";
				break;
			case 185:
				s = "invalid Primary Expression";
				break;
			case 186:
				s = "invalid Type Argument List";
				break;
			case 187:
				s = "invalid Relational Expression";
				break;
			case 188:
				s = "invalid Relational Expression";
				break;
			case 189:
				s = "invalid Type Parameter Constraints Clause Base";
				break;
			default:
				s = "error " + errorNumber;
				break;
			}
			base.Errors.Error(line, col, s);
		}

		private bool StartOf(int s)
		{
			return Parser.set[s, this.lexer.LookAhead.kind];
		}

		public Parser(ILexer lexer) : base(lexer)
		{
			this.lexer = (Lexer)lexer;
		}

		public void Error(string s)
		{
			if (this.errDist >= 2)
			{
				base.Errors.Error(this.lexer.Token.line, this.lexer.Token.col, s);
			}
			this.errDist = 0;
		}

		public override Expression ParseExpression()
		{
			this.lexer.NextToken();
			Expression expr;
			this.Expr(out expr);
			if (this.la.kind == 11)
			{
				this.lexer.NextToken();
			}
			base.Expect(0);
			return expr;
		}

		public override BlockStatement ParseBlock()
		{
			this.lexer.NextToken();
			this.compilationUnit = new CompilationUnit();
			BlockStatement blockStmt = new BlockStatement();
			blockStmt.StartLocation = this.la.Location;
			this.compilationUnit.BlockStart(blockStmt);
			BlockStatement result;
			while (this.la.kind != 0)
			{
				Token oldLa = this.la;
				this.Statement();
				if (this.la == oldLa)
				{
					result = null;
					return result;
				}
			}
			this.compilationUnit.BlockEnd();
			base.Expect(0);
			result = blockStmt;
			return result;
		}

		public override List<INode> ParseTypeMembers()
		{
			this.lexer.NextToken();
			this.compilationUnit = new CompilationUnit();
			TypeDeclaration newType = new TypeDeclaration(Modifiers.None, null);
			this.compilationUnit.BlockStart(newType);
			this.ClassBody();
			this.compilationUnit.BlockEnd();
			base.Expect(0);
			return newType.Children;
		}

		private bool IsTypeCast()
		{
			return this.la.kind == 20 && (this.IsSimpleTypeCast() || this.GuessTypeCast());
		}

		private bool IsSimpleTypeCast()
		{
			this.lexer.StartPeek();
			Token pt = this.lexer.Peek();
			bool result;
			if (!this.IsTypeKWForTypeCast(ref pt))
			{
				result = false;
			}
			else
			{
				if (pt.kind == 12)
				{
					pt = this.lexer.Peek();
				}
				result = (pt.kind == 21);
			}
			return result;
		}

		private bool IsTypeKWForTypeCast(ref Token pt)
		{
			bool result;
			if (Tokens.TypeKW[pt.kind])
			{
				pt = this.lexer.Peek();
				result = (this.IsPointerOrDims(ref pt) && this.SkipQuestionMark(ref pt));
			}
			else if (pt.kind == 122)
			{
				pt = this.lexer.Peek();
				result = this.IsPointerOrDims(ref pt);
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool IsTypeNameOrKWForTypeCast(ref Token pt)
		{
			return this.IsTypeKWForTypeCast(ref pt) || this.IsTypeNameForTypeCast(ref pt);
		}

		private bool IsTypeNameForTypeCast(ref Token pt)
		{
			bool result;
			if (pt.kind != 1)
			{
				result = false;
			}
			else
			{
				pt = this.Peek();
				if (pt.kind == 10)
				{
					pt = this.Peek();
					if (pt.kind != 1)
					{
						result = false;
						return result;
					}
					pt = this.Peek();
				}
				while (true)
				{
					if (pt.kind == 23)
					{
						do
						{
							pt = this.Peek();
							if (!this.IsTypeNameOrKWForTypeCast(ref pt))
							{
								goto Block_4;
							}
						}
						while (pt.kind == 14);
						if (pt.kind != 22)
						{
							goto Block_6;
						}
						pt = this.Peek();
					}
					if (pt.kind != 15)
					{
						goto Block_7;
					}
					pt = this.Peek();
					if (pt.kind != 1)
					{
						goto Block_8;
					}
					pt = this.Peek();
				}
				Block_4:
				result = false;
				return result;
				Block_6:
				result = false;
				return result;
				Block_7:
				if (pt.kind == 12)
				{
					pt = this.Peek();
				}
				result = ((pt.kind != 6 && pt.kind != 18) || this.IsPointerOrDims(ref pt));
				return result;
				Block_8:
				result = false;
			}
			return result;
		}

		private bool GuessTypeCast()
		{
			this.StartPeek();
			Token pt = this.Peek();
			bool result;
			if (!this.IsTypeNameForTypeCast(ref pt))
			{
				result = false;
			}
			else if (pt.kind != 21)
			{
				result = false;
			}
			else
			{
				pt = this.Peek();
				result = (Tokens.CastFollower[pt.kind] || (Tokens.TypeKW[pt.kind] && this.lexer.Peek().kind == 15));
			}
			return result;
		}

		private bool IsQualident(ref Token pt, out string qualident)
		{
			bool result;
			if (pt.kind == 1)
			{
				this.qualidentBuilder.Length = 0;
				this.qualidentBuilder.Append(pt.val);
				pt = this.Peek();
				while (pt.kind == 15 || pt.kind == 10)
				{
					pt = this.Peek();
					if (pt.kind != 1)
					{
						qualident = string.Empty;
						result = false;
						return result;
					}
					this.qualidentBuilder.Append('.');
					this.qualidentBuilder.Append(pt.val);
					pt = this.Peek();
				}
				qualident = this.qualidentBuilder.ToString();
				result = true;
			}
			else
			{
				qualident = string.Empty;
				result = false;
			}
			return result;
		}

		private bool IsPointerOrDims(ref Token pt)
		{
			while (true)
			{
				if (pt.kind == 18)
				{
					do
					{
						pt = this.Peek();
					}
					while (pt.kind == 14);
					if (pt.kind != 19)
					{
						break;
					}
				}
				else if (pt.kind != 6)
				{
					goto Block_3;
				}
				pt = this.Peek();
			}
			bool result = false;
			return result;
			Block_3:
			result = true;
			return result;
		}

		private void StartPeek()
		{
			this.lexer.StartPeek();
		}

		private Token Peek()
		{
			return this.lexer.Peek();
		}

		private Token Peek(int n)
		{
			this.lexer.StartPeek();
			Token x = this.la;
			while (n > 0)
			{
				x = this.lexer.Peek();
				n--;
			}
			return x;
		}

		private bool IdentAndAsgn()
		{
			return this.la.kind == 1 && this.Peek(1).kind == 3;
		}

		private bool IsAssignment()
		{
			return this.IdentAndAsgn();
		}

		private bool IsVarDecl()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 1 && (peek == 14 || peek == 3 || peek == 11 || peek == 18);
		}

		private bool NotFinalComma()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 14 && peek != 17 && peek != 19;
		}

		private bool NotVoidPointer()
		{
			return this.la.kind == 122 && this.Peek(1).kind != 6;
		}

		private bool UnCheckedAndLBrace()
		{
			return this.la.kind == 57 || (this.la.kind == 117 && this.Peek(1).kind == 16);
		}

		private bool DotAndIdent()
		{
			return this.la.kind == 15 && this.Peek(1).kind == 1;
		}

		private bool IdentAndColon()
		{
			return this.la.kind == 1 && this.Peek(1).kind == 9;
		}

		private bool IsLabel()
		{
			return this.IdentAndColon();
		}

		private bool IdentAndLPar()
		{
			return this.la.kind == 1 && this.Peek(1).kind == 20;
		}

		private bool CatchAndLPar()
		{
			return this.la.kind == 55 && this.Peek(1).kind == 20;
		}

		private bool IsTypedCatch()
		{
			return this.CatchAndLPar();
		}

		private bool IsGlobalAttrTarget()
		{
			Token pt = this.Peek(1);
			return this.la.kind == 18 && pt.kind == 1 && pt.val == "assembly";
		}

		private bool LBrackAndCommaOrRBrack()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 18 && (peek == 14 || peek == 19);
		}

		private bool TimesOrLBrackAndCommaOrRBrack()
		{
			return this.la.kind == 6 || this.LBrackAndCommaOrRBrack();
		}

		private bool IsPointerOrDims()
		{
			return this.TimesOrLBrackAndCommaOrRBrack();
		}

		private bool IsPointer()
		{
			return this.la.kind == 6;
		}

		private bool SkipGeneric(ref Token pt)
		{
			bool result;
			if (pt.kind == 23)
			{
				while (true)
				{
					pt = this.Peek();
					if (!this.IsTypeNameOrKWForTypeCast(ref pt))
					{
						break;
					}
					if (pt.kind != 14)
					{
						goto Block_2;
					}
				}
				result = false;
				return result;
				Block_2:
				if (pt.kind != 22)
				{
					result = false;
					return result;
				}
				pt = this.Peek();
			}
			result = true;
			return result;
		}

		private bool SkipQuestionMark(ref Token pt)
		{
			if (pt.kind == 12)
			{
				pt = this.Peek();
			}
			return true;
		}

		private bool IsLocalVarDecl()
		{
			bool result;
			if (this.IsYieldStatement())
			{
				result = false;
			}
			else if ((Tokens.TypeKW[this.la.kind] && this.Peek(1).kind != 15) || this.la.kind == 122)
			{
				result = true;
			}
			else
			{
				this.StartPeek();
				Token pt = this.la;
				result = (this.IsTypeNameOrKWForTypeCast(ref pt) && pt.kind == 1);
			}
			return result;
		}

		private bool IsGenericFollowedBy(int token)
		{
			Token t = this.la;
			bool result;
			if (t.kind != 23)
			{
				result = false;
			}
			else
			{
				this.StartPeek();
				result = (this.SkipGeneric(ref t) && t.kind == token);
			}
			return result;
		}

		private bool IsExplicitInterfaceImplementation()
		{
			this.StartPeek();
			Token pt = this.la;
			pt = this.Peek();
			bool result;
			if (pt.kind == 15 || pt.kind == 10)
			{
				result = true;
			}
			else
			{
				if (pt.kind == 23)
				{
					if (this.SkipGeneric(ref pt))
					{
						result = (pt.kind == 15);
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		private bool IdentIsWhere()
		{
			return this.la.kind == 1 && this.la.val == "where";
		}

		private bool IdentIsGet()
		{
			return this.la.kind == 1 && this.la.val == "get";
		}

		private bool IdentIsSet()
		{
			return this.la.kind == 1 && this.la.val == "set";
		}

		private bool IdentIsAdd()
		{
			return this.la.kind == 1 && this.la.val == "add";
		}

		private bool IdentIsRemove()
		{
			return this.la.kind == 1 && this.la.val == "remove";
		}

		private bool IsYieldStatement()
		{
			return this.la.kind == 1 && this.la.val == "yield" && (this.Peek(1).kind == 100 || this.Peek(1).kind == 52);
		}

		private bool IsLocalAttrTarget()
		{
			int cur = this.la.kind;
			string val = this.la.val;
			return (cur == 68 || cur == 100 || (cur == 1 && (val == "field" || val == "method" || val == "module" || val == "param" || val == "property" || val == "type"))) && this.Peek(1).kind == 9;
		}

		private bool IsShiftRight()
		{
			Token next = this.Peek(1);
			return this.la.kind == 22 && next.kind == 22;
		}

		private bool IsTypeReferenceExpression(Expression expr)
		{
			bool result;
			if (expr is TypeReferenceExpression)
			{
				result = (((TypeReferenceExpression)expr).TypeReference.GenericTypes.Count == 0);
			}
			else
			{
				while (expr is FieldReferenceExpression)
				{
					expr = ((FieldReferenceExpression)expr).TargetObject;
					if (expr is TypeReferenceExpression)
					{
						result = true;
						return result;
					}
				}
				result = (expr is IdentifierExpression);
			}
			return result;
		}

		private TypeReferenceExpression GetTypeReferenceExpression(Expression expr, List<TypeReference> genericTypes)
		{
			TypeReferenceExpression tre = expr as TypeReferenceExpression;
			TypeReferenceExpression result;
			if (tre != null)
			{
				result = new TypeReferenceExpression(new TypeReference(tre.TypeReference.Type, tre.TypeReference.PointerNestingLevel, tre.TypeReference.RankSpecifier, genericTypes));
			}
			else
			{
				StringBuilder b = new StringBuilder();
				if (!this.WriteFullTypeName(b, expr))
				{
					while (expr is FieldReferenceExpression)
					{
						expr = ((FieldReferenceExpression)expr).TargetObject;
					}
					tre = (expr as TypeReferenceExpression);
					if (tre != null)
					{
						TypeReference typeRef = tre.TypeReference;
						if (typeRef.GenericTypes.Count == 0)
						{
							typeRef = typeRef.Clone();
							TypeReference expr_B6 = typeRef;
							expr_B6.Type = expr_B6.Type + "." + b.ToString();
							typeRef.GenericTypes.AddRange(genericTypes);
						}
						else
						{
							typeRef = new InnerClassTypeReference(typeRef, b.ToString(), genericTypes);
						}
						result = new TypeReferenceExpression(typeRef);
						return result;
					}
				}
				result = new TypeReferenceExpression(new TypeReference(b.ToString(), 0, null, genericTypes));
			}
			return result;
		}

		private bool WriteFullTypeName(StringBuilder b, Expression expr)
		{
			FieldReferenceExpression fre = expr as FieldReferenceExpression;
			bool result2;
			if (fre != null)
			{
				bool result = this.WriteFullTypeName(b, fre.TargetObject);
				if (b.Length > 0)
				{
					b.Append('.');
				}
				b.Append(fre.FieldName);
				result2 = result;
			}
			else if (expr is IdentifierExpression)
			{
				b.Append(((IdentifierExpression)expr).Identifier);
				result2 = true;
			}
			else
			{
				result2 = false;
			}
			return result2;
		}

		private bool IsMostNegativeIntegerWithoutTypeSuffix()
		{
			Token token = this.la;
			return token.kind == 2 && (token.val == "2147483648" || token.val == "9223372036854775808");
		}

		private bool LastExpressionIsUnaryMinus(ArrayList expressions)
		{
			bool result;
			if (expressions.Count == 0)
			{
				result = false;
			}
			else
			{
				UnaryOperatorExpression uoe = expressions[expressions.Count - 1] as UnaryOperatorExpression;
				result = (uoe != null && uoe.Op == UnaryOperatorType.Minus);
			}
			return result;
		}
	}
}
