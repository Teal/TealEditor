using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.VB
{
	internal sealed class Parser : AbstractParser
	{
		private const int maxT = 205;

		private const bool T = true;

		private const bool x = false;

		private Lexer lexer;

		private StringBuilder qualidentBuilder = new StringBuilder();

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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				true,
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
				true,
				true,
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
				true,
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
				true,
				true,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				true,
				false,
				false,
				false
			},
			{
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
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
				false,
				false
			},
			{
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
				false,
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
				false,
				false,
				false,
				true,
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
				false,
				true,
				false,
				false
			},
			{
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
				true,
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
				false,
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
				false
			},
			{
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
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				true,
				false,
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
				false
			},
			{
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				true,
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
				true,
				true,
				true,
				false,
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
				true,
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
				false,
				false,
				false,
				false,
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
				false
			},
			{
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
				false,
				false
			},
			{
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				true,
				true,
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
				true,
				true,
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
				true,
				true,
				true,
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
				true,
				true,
				false,
				true,
				false,
				false,
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
				true,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
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
				true,
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
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false
			},
			{
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				true,
				true,
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
				true,
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
				true,
				true,
				false,
				true,
				false,
				false,
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
				true,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
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
				true,
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
				false,
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
				true,
				true,
				false,
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
				true,
				true,
				true,
				false,
				false,
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
				true,
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
				true,
				true,
				false,
				true,
				false,
				false,
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
				true,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
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
				true,
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
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				false
			},
			{
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
				true,
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
				true,
				false,
				true,
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
				true,
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
				true,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
				true,
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
				true,
				false,
				true,
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
				true,
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
				true,
				false,
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
				true,
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
				true,
				false,
				true,
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
				true,
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
				true,
				false,
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
				true,
				true,
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
				true,
				false,
				true,
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
				true,
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
				true,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false
			},
			{
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
				true,
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
				true,
				false,
				true,
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
				true,
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
				true,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false
			},
			{
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
				false,
				true,
				true,
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
				true,
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
				true,
				true,
				false,
				true,
				false,
				false,
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
				true,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
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
				true,
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
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false
			},
			{
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
				false,
				true,
				true,
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
				true,
				true,
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
				true,
				true,
				true,
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
				true,
				true,
				false,
				true,
				false,
				false,
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
				true,
				false,
				false,
				false,
				true,
				true,
				false,
				false,
				true,
				false,
				true,
				false,
				false,
				true,
				true,
				false,
				true,
				false,
				true,
				true,
				true,
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
				true,
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
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				false,
				false,
				false,
				false,
				false,
				false,
				false,
				false,
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
				true,
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
				false,
				false,
				false,
				false,
				false,
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

		public Parser(ILexer lexer) : base(lexer)
		{
			this.lexer = (Lexer)lexer;
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

		public void Error(string s)
		{
			if (this.errDist >= 2)
			{
				base.Errors.Error(this.la.line, this.la.col, s);
			}
			this.errDist = 0;
		}

		public override Expression ParseExpression()
		{
			this.lexer.NextToken();
			Expression expr;
			this.Expr(out expr);
			while (this.la.kind == 1)
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
			Statement st;
			this.Block(out st);
			base.Expect(0);
			return st as BlockStatement;
		}

		public override List<INode> ParseTypeMembers()
		{
			this.lexer.NextToken();
			this.compilationUnit = new CompilationUnit();
			TypeDeclaration newType = new TypeDeclaration(Modifiers.None, null);
			this.compilationUnit.BlockStart(newType);
			this.ClassBody(newType);
			this.compilationUnit.BlockEnd();
			base.Expect(0);
			return newType.Children;
		}

		private bool LeaveBlock()
		{
			int peek = this.Peek(1).kind;
			return Tokens.BlockSucc[this.la.kind] && (this.la.kind != 88 || peek == 1 || peek == 13);
		}

		private bool DotAndIdentOrKw()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 10 && (peek == 2 || peek >= 42);
		}

		private bool IsEndStmtAhead()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 88 && (peek == 1 || peek == 13);
		}

		private bool IsNotClosingParenthesis()
		{
			return this.la.kind != 25;
		}

		private bool IsNamedAssign()
		{
			return this.Peek(1).kind == 13 && this.Peek(2).kind == 11;
		}

		private bool IsObjectCreation()
		{
			return this.la.kind == 48 && this.Peek(1).kind == 127;
		}

		private bool IsGlobalAttrTarget()
		{
			Token pt = this.Peek(1);
			return this.la.kind == 27 && (string.Equals(pt.val, "assembly", StringComparison.InvariantCultureIgnoreCase) || string.Equals(pt.val, "module", StringComparison.InvariantCultureIgnoreCase));
		}

		private bool IsDims()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 24 && (peek == 12 || peek == 25);
		}

		private bool IsSize()
		{
			return this.la.kind == 24;
		}

		private bool NotFinalComma()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 12 && peek != 23;
		}

		private bool IsElseIf()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 86 && peek == 106;
		}

		private bool IsNegativeLabelName()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 104 && peek == 15;
		}

		private bool IsResumeNext()
		{
			int peek = this.Peek(1).kind;
			return this.la.kind == 153 && peek == 128;
		}

		private bool IsLabel()
		{
			return (this.la.kind == 2 || this.la.kind == 5) && this.Peek(1).kind == 13;
		}

		private bool IsNotStatementSeparator()
		{
			return this.la.kind == 13 && this.Peek(1).kind == 1;
		}

		private static bool IsMustOverride(ModifierList m)
		{
			return m.Contains(Modifiers.Dim);
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

		private bool IsLocalAttrTarget()
		{
			return false;
		}

		private void EnsureIsZero(Expression expr)
		{
			if (!(expr is PrimitiveExpression) || (expr as PrimitiveExpression).StringValue != "0")
			{
				this.Error("lower bound of array must be zero");
			}
		}

		private void VBNET()
		{
			this.lexer.NextToken();
			this.compilationUnit = new CompilationUnit();
			while (this.la.kind == 1)
			{
				this.lexer.NextToken();
			}
			while (this.la.kind == 136)
			{
				this.OptionStmt();
			}
			while (this.la.kind == 108)
			{
				this.ImportsStmt();
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

		private void OptionStmt()
		{
			INode node = null;
			bool val = true;
			base.Expect(136);
			Location startPos = this.t.Location;
			if (this.la.kind == 95)
			{
				this.lexer.NextToken();
				if (this.la.kind == 134 || this.la.kind == 135)
				{
					this.OptionValue(ref val);
				}
				node = new OptionDeclaration(OptionType.Explicit, val);
			}
			else if (this.la.kind == 164)
			{
				this.lexer.NextToken();
				if (this.la.kind == 134 || this.la.kind == 135)
				{
					this.OptionValue(ref val);
				}
				node = new OptionDeclaration(OptionType.Strict, val);
			}
			else if (this.la.kind == 70)
			{
				this.lexer.NextToken();
				if (this.la.kind == 51)
				{
					this.lexer.NextToken();
					node = new OptionDeclaration(OptionType.CompareBinary, val);
				}
				else if (this.la.kind == 169)
				{
					this.lexer.NextToken();
					node = new OptionDeclaration(OptionType.CompareText, val);
				}
				else
				{
					base.SynErr(206);
				}
			}
			else
			{
				base.SynErr(207);
			}
			this.EndOfStmt();
			if (node != null)
			{
				node.StartLocation = startPos;
				node.EndLocation = this.t.Location;
				this.compilationUnit.AddChild(node);
			}
		}

		private void ImportsStmt()
		{
			List<Using> usings = new List<Using>();
			base.Expect(108);
			Location startPos = this.t.Location;
			Using u;
			this.ImportClause(out u);
			if (u != null)
			{
				usings.Add(u);
			}
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.ImportClause(out u);
				if (u != null)
				{
					usings.Add(u);
				}
			}
			this.EndOfStmt();
			UsingDeclaration usingDeclaration = new UsingDeclaration(usings);
			usingDeclaration.StartLocation = startPos;
			usingDeclaration.EndLocation = this.t.Location;
			this.compilationUnit.AddChild(usingDeclaration);
		}

		private void GlobalAttributeSection()
		{
			base.Expect(27);
			Location startPos = this.t.Location;
			if (this.la.kind == 49)
			{
				this.lexer.NextToken();
			}
			else if (this.la.kind == 121)
			{
				this.lexer.NextToken();
			}
			else
			{
				base.SynErr(208);
			}
			string attributeTarget = this.t.val.ToLower(CultureInfo.InvariantCulture);
			List<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute> attributes = new List<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute>();
			base.Expect(13);
			AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute;
			this.Attribute(out attribute);
			attributes.Add(attribute);
			while (this.NotFinalComma())
			{
				if (this.la.kind == 12)
				{
					this.lexer.NextToken();
					if (this.la.kind == 49)
					{
						this.lexer.NextToken();
					}
					else if (this.la.kind == 121)
					{
						this.lexer.NextToken();
					}
					else
					{
						base.SynErr(209);
					}
					base.Expect(13);
				}
				this.Attribute(out attribute);
				attributes.Add(attribute);
			}
			if (this.la.kind == 12)
			{
				this.lexer.NextToken();
			}
			base.Expect(26);
			this.EndOfStmt();
			AttributeSection section = new AttributeSection(attributeTarget, attributes);
			section.StartLocation = startPos;
			section.EndLocation = this.t.EndLocation;
			this.compilationUnit.AddChild(section);
		}

		private void NamespaceMemberDecl()
		{
			ModifierList i = new ModifierList();
			List<AttributeSection> attributes = new List<AttributeSection>();
			if (this.la.kind == 126)
			{
				this.lexer.NextToken();
				Location startPos = this.t.Location;
				string qualident;
				this.Qualident(out qualident);
				INode node = new NamespaceDeclaration(qualident);
				node.StartLocation = startPos;
				this.compilationUnit.AddChild(node);
				this.compilationUnit.BlockStart(node);
				base.Expect(1);
				this.NamespaceBody();
				node.EndLocation = this.t.Location;
				this.compilationUnit.BlockEnd();
			}
			else if (this.StartOf(2))
			{
				while (this.la.kind == 27)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				while (this.StartOf(3))
				{
					this.TypeModifier(i);
				}
				this.NonModuleDeclaration(i, attributes);
			}
			else
			{
				base.SynErr(210);
			}
		}

		private void OptionValue(ref bool val)
		{
			if (this.la.kind == 135)
			{
				this.lexer.NextToken();
				val = true;
			}
			else if (this.la.kind == 134)
			{
				this.lexer.NextToken();
				val = false;
			}
			else
			{
				base.SynErr(211);
			}
		}

		private void EndOfStmt()
		{
			if (this.la.kind == 1)
			{
				this.lexer.NextToken();
			}
			else if (this.la.kind == 13)
			{
				this.lexer.NextToken();
				if (this.la.kind == 1)
				{
					this.lexer.NextToken();
				}
			}
			else
			{
				base.SynErr(212);
			}
		}

		private void ImportClause(out Using u)
		{
			string qualident = null;
			TypeReference aliasedType = null;
			u = null;
			this.Qualident(out qualident);
			if (this.la.kind == 11)
			{
				this.lexer.NextToken();
				this.TypeName(out aliasedType);
			}
			if (qualident != null && qualident.Length > 0)
			{
				if (aliasedType != null)
				{
					u = new Using(qualident, aliasedType);
				}
				else
				{
					u = new Using(qualident);
				}
			}
		}

		private void Qualident(out string qualident)
		{
			this.qualidentBuilder.Length = 0;
			this.Identifier();
			this.qualidentBuilder.Append(this.t.val);
			while (this.DotAndIdentOrKw())
			{
				base.Expect(10);
				string name;
				this.IdentifierOrKeyword(out name);
				this.qualidentBuilder.Append('.');
				this.qualidentBuilder.Append(name);
			}
			qualident = this.qualidentBuilder.ToString();
		}

		private void TypeName(out TypeReference typeref)
		{
			ArrayList rank = null;
			this.NonArrayTypeName(out typeref, false);
			this.ArrayTypeModifiers(out rank);
			if (rank != null && typeref != null)
			{
				typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
			}
		}

		private void NamespaceBody()
		{
			while (this.StartOf(1))
			{
				this.NamespaceMemberDecl();
			}
			base.Expect(88);
			base.Expect(126);
			base.Expect(1);
		}

		private void AttributeSection(out AttributeSection section)
		{
			string attributeTarget = "";
			List<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute> attributes = new List<AIMS.Libraries.Scripting.NRefactory.Ast.Attribute>();
			base.Expect(27);
			Location startPos = this.t.Location;
			if (this.IsLocalAttrTarget())
			{
				if (this.la.kind == 93)
				{
					this.lexer.NextToken();
					attributeTarget = "event";
				}
				else if (this.la.kind == 154)
				{
					this.lexer.NextToken();
					attributeTarget = "return";
				}
				else
				{
					this.Identifier();
					string val = this.t.val.ToLower(CultureInfo.InvariantCulture);
					if (val != "field" || val != "method" || val != "module" || val != "param" || val != "property" || val != "type")
					{
						this.Error("attribute target specifier (event, return, field,method, module, param, property, or type) expected");
					}
					attributeTarget = this.t.val;
				}
				base.Expect(13);
			}
			AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute;
			this.Attribute(out attribute);
			attributes.Add(attribute);
			while (this.NotFinalComma())
			{
				base.Expect(12);
				this.Attribute(out attribute);
				attributes.Add(attribute);
			}
			if (this.la.kind == 12)
			{
				this.lexer.NextToken();
			}
			base.Expect(26);
			section = new AttributeSection(attributeTarget, attributes);
			section.StartLocation = startPos;
			section.EndLocation = this.t.EndLocation;
		}

		private void TypeModifier(ModifierList m)
		{
			int kind = this.la.kind;
			if (kind <= 131)
			{
				if (kind == 99)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Internal, this.t.Location);
					return;
				}
				if (kind == 122)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Dim, this.t.Location);
					return;
				}
				if (kind == 131)
				{
					this.lexer.NextToken();
					m.Add(Modifiers.Sealed, this.t.Location);
					return;
				}
			}
			else
			{
				switch (kind)
				{
				case 145:
					this.lexer.NextToken();
					m.Add(Modifiers.Private, this.t.Location);
					return;
				case 146:
					break;
				case 147:
					this.lexer.NextToken();
					m.Add(Modifiers.Protected, this.t.Location);
					return;
				case 148:
					this.lexer.NextToken();
					m.Add(Modifiers.Public, this.t.Location);
					return;
				default:
					switch (kind)
					{
					case 157:
						this.lexer.NextToken();
						m.Add(Modifiers.New, this.t.Location);
						return;
					case 158:
						this.lexer.NextToken();
						m.Add(Modifiers.Static, this.t.Location);
						return;
					default:
						if (kind == 203)
						{
							this.lexer.NextToken();
							m.Add(Modifiers.Partial, this.t.Location);
							return;
						}
						break;
					}
					break;
				}
			}
			base.SynErr(213);
		}

		private void NonModuleDeclaration(ModifierList m, List<AttributeSection> attributes)
		{
			TypeReference typeRef = null;
			List<TypeReference> baseInterfaces = null;
			int kind = this.la.kind;
			if (kind <= 90)
			{
				if (kind == 67)
				{
					m.Check(Modifiers.Classes);
					this.lexer.NextToken();
					TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
					newType.StartLocation = this.t.Location;
					this.compilationUnit.AddChild(newType);
					this.compilationUnit.BlockStart(newType);
					newType.Type = ClassType.Class;
					this.Identifier();
					newType.Name = this.t.val;
					this.TypeParameterList(newType.Templates);
					this.EndOfStmt();
					newType.BodyStartLocation = this.t.Location;
					if (this.la.kind == 110)
					{
						this.ClassBaseType(out typeRef);
						newType.BaseTypes.Add(typeRef);
					}
					while (this.la.kind == 107)
					{
						this.TypeImplementsClause(out baseInterfaces);
						newType.BaseTypes.AddRange(baseInterfaces);
					}
					this.ClassBody(newType);
					base.Expect(88);
					base.Expect(67);
					newType.EndLocation = this.t.EndLocation;
					base.Expect(1);
					this.compilationUnit.BlockEnd();
					return;
				}
				if (kind == 80)
				{
					this.lexer.NextToken();
					m.Check(Modifiers.VBStructures);
					DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
					delegateDeclr.ReturnType = new TypeReference("", "System.Void");
					delegateDeclr.StartLocation = m.GetDeclarationLocation(this.t.Location);
					List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
					if (this.la.kind == 167)
					{
						this.lexer.NextToken();
						this.Identifier();
						delegateDeclr.Name = this.t.val;
						this.TypeParameterList(delegateDeclr.Templates);
						if (this.la.kind == 24)
						{
							this.lexer.NextToken();
							if (this.StartOf(4))
							{
								this.FormalParameterList(p);
							}
							base.Expect(25);
							delegateDeclr.Parameters = p;
						}
					}
					else if (this.la.kind == 100)
					{
						this.lexer.NextToken();
						this.Identifier();
						delegateDeclr.Name = this.t.val;
						this.TypeParameterList(delegateDeclr.Templates);
						if (this.la.kind == 24)
						{
							this.lexer.NextToken();
							if (this.StartOf(4))
							{
								this.FormalParameterList(p);
							}
							base.Expect(25);
							delegateDeclr.Parameters = p;
						}
						if (this.la.kind == 48)
						{
							this.lexer.NextToken();
							TypeReference type;
							this.TypeName(out type);
							delegateDeclr.ReturnType = type;
						}
					}
					else
					{
						base.SynErr(214);
					}
					delegateDeclr.EndLocation = this.t.EndLocation;
					base.Expect(1);
					this.compilationUnit.AddChild(delegateDeclr);
					return;
				}
				if (kind == 90)
				{
					this.lexer.NextToken();
					m.Check(Modifiers.VBStructures);
					TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
					newType.StartLocation = m.GetDeclarationLocation(this.t.Location);
					this.compilationUnit.AddChild(newType);
					this.compilationUnit.BlockStart(newType);
					newType.Type = ClassType.Enum;
					this.Identifier();
					newType.Name = this.t.val;
					if (this.la.kind == 48)
					{
						this.lexer.NextToken();
						this.NonArrayTypeName(out typeRef, false);
						newType.BaseTypes.Add(typeRef);
					}
					base.Expect(1);
					newType.BodyStartLocation = this.t.Location;
					this.EnumBody(newType);
					this.compilationUnit.BlockEnd();
					return;
				}
			}
			else
			{
				if (kind == 112)
				{
					this.lexer.NextToken();
					m.Check(Modifiers.VBStructures);
					TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
					newType.StartLocation = m.GetDeclarationLocation(this.t.Location);
					this.compilationUnit.AddChild(newType);
					this.compilationUnit.BlockStart(newType);
					newType.Type = ClassType.Interface;
					this.Identifier();
					newType.Name = this.t.val;
					this.TypeParameterList(newType.Templates);
					this.EndOfStmt();
					newType.BodyStartLocation = this.t.Location;
					while (this.la.kind == 110)
					{
						this.InterfaceBase(out baseInterfaces);
						newType.BaseTypes.AddRange(baseInterfaces);
					}
					this.InterfaceBody(newType);
					this.compilationUnit.BlockEnd();
					return;
				}
				if (kind == 121)
				{
					this.lexer.NextToken();
					m.Check(Modifiers.Visibility);
					TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
					this.compilationUnit.AddChild(newType);
					this.compilationUnit.BlockStart(newType);
					newType.StartLocation = m.GetDeclarationLocation(this.t.Location);
					newType.Type = ClassType.Module;
					this.Identifier();
					newType.Name = this.t.val;
					base.Expect(1);
					newType.BodyStartLocation = this.t.Location;
					this.ModuleBody(newType);
					this.compilationUnit.BlockEnd();
					return;
				}
				if (kind == 166)
				{
					this.lexer.NextToken();
					m.Check(Modifiers.VBStructures);
					TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
					this.compilationUnit.AddChild(newType);
					this.compilationUnit.BlockStart(newType);
					newType.StartLocation = m.GetDeclarationLocation(this.t.Location);
					newType.Type = ClassType.Struct;
					this.Identifier();
					newType.Name = this.t.val;
					this.TypeParameterList(newType.Templates);
					base.Expect(1);
					newType.BodyStartLocation = this.t.Location;
					while (this.la.kind == 107)
					{
						this.TypeImplementsClause(out baseInterfaces);
						newType.BaseTypes.AddRange(baseInterfaces);
					}
					this.StructureBody(newType);
					this.compilationUnit.BlockEnd();
					return;
				}
			}
			base.SynErr(215);
		}

		private void TypeParameterList(List<TemplateDefinition> templates)
		{
			if (this.la.kind == 24 && this.Peek(1).kind == 200)
			{
				this.lexer.NextToken();
				base.Expect(200);
				TemplateDefinition template;
				this.TypeParameter(out template);
				if (template != null)
				{
					templates.Add(template);
				}
				while (this.la.kind == 12)
				{
					this.lexer.NextToken();
					this.TypeParameter(out template);
					if (template != null)
					{
						templates.Add(template);
					}
				}
				base.Expect(25);
			}
		}

		private void TypeParameter(out TemplateDefinition template)
		{
			this.Identifier();
			template = new TemplateDefinition(this.t.val, null);
			if (this.la.kind == 48)
			{
				this.TypeParameterConstraints(template);
			}
		}

		private void Identifier()
		{
			int kind = this.la.kind;
			if (kind <= 95)
			{
				if (kind <= 51)
				{
					if (kind == 2)
					{
						this.lexer.NextToken();
						return;
					}
					switch (kind)
					{
					case 47:
						this.lexer.NextToken();
						return;
					case 49:
						this.lexer.NextToken();
						return;
					case 50:
						this.lexer.NextToken();
						return;
					case 51:
						this.lexer.NextToken();
						return;
					}
				}
				else
				{
					if (kind == 70)
					{
						this.lexer.NextToken();
						return;
					}
					if (kind == 95)
					{
						this.lexer.NextToken();
						return;
					}
				}
			}
			else if (kind <= 144)
			{
				if (kind == 134)
				{
					this.lexer.NextToken();
					return;
				}
				if (kind == 144)
				{
					this.lexer.NextToken();
					return;
				}
			}
			else
			{
				if (kind == 169)
				{
					this.lexer.NextToken();
					return;
				}
				switch (kind)
				{
				case 176:
					this.lexer.NextToken();
					return;
				case 177:
					this.lexer.NextToken();
					return;
				default:
					if (kind == 204)
					{
						this.lexer.NextToken();
						return;
					}
					break;
				}
			}
			base.SynErr(216);
		}

		private void TypeParameterConstraints(TemplateDefinition template)
		{
			base.Expect(48);
			if (this.la.kind == 22)
			{
				this.lexer.NextToken();
				TypeReference constraint;
				this.TypeParameterConstraint(out constraint);
				if (constraint != null)
				{
					template.Bases.Add(constraint);
				}
				while (this.la.kind == 12)
				{
					this.lexer.NextToken();
					this.TypeParameterConstraint(out constraint);
					if (constraint != null)
					{
						template.Bases.Add(constraint);
					}
				}
				base.Expect(23);
			}
			else if (this.StartOf(5))
			{
				TypeReference constraint;
				this.TypeParameterConstraint(out constraint);
				if (constraint != null)
				{
					template.Bases.Add(constraint);
				}
			}
			else
			{
				base.SynErr(217);
			}
		}

		private void TypeParameterConstraint(out TypeReference constraint)
		{
			constraint = null;
			if (this.la.kind == 67)
			{
				this.lexer.NextToken();
				constraint = TypeReference.ClassConstraint;
			}
			else if (this.la.kind == 166)
			{
				this.lexer.NextToken();
				constraint = TypeReference.StructConstraint;
			}
			else if (this.la.kind == 127)
			{
				this.lexer.NextToken();
				constraint = TypeReference.NewConstraint;
			}
			else if (this.StartOf(6))
			{
				this.TypeName(out constraint);
			}
			else
			{
				base.SynErr(218);
			}
		}

		private void ClassBaseType(out TypeReference typeRef)
		{
			typeRef = null;
			base.Expect(110);
			this.TypeName(out typeRef);
			this.EndOfStmt();
		}

		private void TypeImplementsClause(out List<TypeReference> baseInterfaces)
		{
			baseInterfaces = new List<TypeReference>();
			TypeReference type = null;
			base.Expect(107);
			this.TypeName(out type);
			baseInterfaces.Add(type);
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.TypeName(out type);
				baseInterfaces.Add(type);
			}
			this.EndOfStmt();
		}

		private void ClassBody(TypeDeclaration newType)
		{
			while (this.StartOf(7))
			{
				List<AttributeSection> attributes = new List<AttributeSection>();
				ModifierList i = new ModifierList();
				while (this.la.kind == 27)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				while (this.StartOf(8))
				{
					this.MemberModifier(i);
				}
				this.ClassMemberDecl(i, attributes);
			}
		}

		private void ModuleBody(TypeDeclaration newType)
		{
			while (this.StartOf(7))
			{
				List<AttributeSection> attributes = new List<AttributeSection>();
				ModifierList i = new ModifierList();
				while (this.la.kind == 27)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				while (this.StartOf(8))
				{
					this.MemberModifier(i);
				}
				this.ClassMemberDecl(i, attributes);
			}
			base.Expect(88);
			base.Expect(121);
			newType.EndLocation = this.t.EndLocation;
			base.Expect(1);
		}

		private void StructureBody(TypeDeclaration newType)
		{
			while (this.StartOf(7))
			{
				List<AttributeSection> attributes = new List<AttributeSection>();
				ModifierList i = new ModifierList();
				while (this.la.kind == 27)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				while (this.StartOf(8))
				{
					this.MemberModifier(i);
				}
				this.StructureMemberDecl(i, attributes);
			}
			base.Expect(88);
			base.Expect(166);
			newType.EndLocation = this.t.EndLocation;
			base.Expect(1);
		}

		private void NonArrayTypeName(out TypeReference typeref, bool canBeUnbound)
		{
			typeref = null;
			bool isGlobal = false;
			if (this.StartOf(9))
			{
				if (this.la.kind == 198)
				{
					this.lexer.NextToken();
					base.Expect(10);
					isGlobal = true;
				}
				this.QualIdentAndTypeArguments(out typeref, canBeUnbound);
				typeref.IsGlobal = isGlobal;
				while (this.la.kind == 10)
				{
					this.lexer.NextToken();
					TypeReference nestedTypeRef;
					this.QualIdentAndTypeArguments(out nestedTypeRef, canBeUnbound);
					typeref = new InnerClassTypeReference(typeref, nestedTypeRef.Type, nestedTypeRef.GenericTypes);
				}
			}
			else if (this.la.kind == 133)
			{
				this.lexer.NextToken();
				typeref = new TypeReference("System.Object");
			}
			else if (this.StartOf(10))
			{
				string name;
				this.PrimitiveTypeName(out name);
				typeref = new TypeReference(name);
			}
			else
			{
				base.SynErr(219);
			}
		}

		private void EnumBody(TypeDeclaration newType)
		{
			while (this.StartOf(11))
			{
				FieldDeclaration f;
				this.EnumMemberDecl(out f);
				this.compilationUnit.AddChild(f);
			}
			base.Expect(88);
			base.Expect(90);
			newType.EndLocation = this.t.EndLocation;
			base.Expect(1);
		}

		private void InterfaceBase(out List<TypeReference> bases)
		{
			bases = new List<TypeReference>();
			base.Expect(110);
			TypeReference type;
			this.TypeName(out type);
			bases.Add(type);
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.TypeName(out type);
				bases.Add(type);
			}
			base.Expect(1);
		}

		private void InterfaceBody(TypeDeclaration newType)
		{
			while (this.StartOf(12))
			{
				this.InterfaceMemberDecl();
			}
			base.Expect(88);
			base.Expect(112);
			newType.EndLocation = this.t.EndLocation;
			base.Expect(1);
		}

		private void FormalParameterList(List<ParameterDeclarationExpression> parameter)
		{
			List<AttributeSection> attributes = new List<AttributeSection>();
			while (this.la.kind == 27)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			ParameterDeclarationExpression p;
			this.FormalParameter(out p);
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				if (paramsFound)
				{
					this.Error("params array must be at end of parameter list");
				}
				while (this.la.kind == 27)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				this.FormalParameter(out p);
				p.Attributes = attributes;
				parameter.Add(p);
			}
		}

		private void MemberModifier(ModifierList m)
		{
			int kind = this.la.kind;
			if (kind <= 123)
			{
				switch (kind)
				{
				case 79:
					this.lexer.NextToken();
					m.Add(Modifiers.Default, this.t.Location);
					return;
				case 80:
					break;
				case 81:
					this.lexer.NextToken();
					m.Add(Modifiers.Dim, this.t.Location);
					return;
				default:
					if (kind == 99)
					{
						this.lexer.NextToken();
						m.Add(Modifiers.Internal, this.t.Location);
						return;
					}
					switch (kind)
					{
					case 122:
						this.lexer.NextToken();
						m.Add(Modifiers.Dim, this.t.Location);
						return;
					case 123:
						this.lexer.NextToken();
						m.Add(Modifiers.Dim, this.t.Location);
						return;
					}
					break;
				}
			}
			else if (kind <= 150)
			{
				switch (kind)
				{
				case 131:
					this.lexer.NextToken();
					m.Add(Modifiers.Sealed, this.t.Location);
					return;
				case 132:
					this.lexer.NextToken();
					m.Add(Modifiers.Sealed, this.t.Location);
					return;
				default:
					switch (kind)
					{
					case 140:
						this.lexer.NextToken();
						m.Add(Modifiers.Overloads, this.t.Location);
						return;
					case 141:
						this.lexer.NextToken();
						m.Add(Modifiers.Virtual, this.t.Location);
						return;
					case 142:
						this.lexer.NextToken();
						m.Add(Modifiers.Override, this.t.Location);
						return;
					case 145:
						this.lexer.NextToken();
						m.Add(Modifiers.Private, this.t.Location);
						return;
					case 147:
						this.lexer.NextToken();
						m.Add(Modifiers.Protected, this.t.Location);
						return;
					case 148:
						this.lexer.NextToken();
						m.Add(Modifiers.Public, this.t.Location);
						return;
					case 150:
						this.lexer.NextToken();
						m.Add(Modifiers.ReadOnly, this.t.Location);
						return;
					}
					break;
				}
			}
			else
			{
				switch (kind)
				{
				case 157:
					this.lexer.NextToken();
					m.Add(Modifiers.New, this.t.Location);
					return;
				case 158:
					this.lexer.NextToken();
					m.Add(Modifiers.Static, this.t.Location);
					return;
				default:
					switch (kind)
					{
					case 183:
						this.lexer.NextToken();
						m.Add(Modifiers.WithEvents, this.t.Location);
						return;
					case 184:
						this.lexer.NextToken();
						m.Add(Modifiers.WriteOnly, this.t.Location);
						return;
					}
					break;
				}
			}
			base.SynErr(220);
		}

		private void ClassMemberDecl(ModifierList m, List<AttributeSection> attributes)
		{
			this.StructureMemberDecl(m, attributes);
		}

		private void StructureMemberDecl(ModifierList m, List<AttributeSection> attributes)
		{
			TypeReference type = null;
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			Statement stmt = null;
			List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
			List<TemplateDefinition> templates = new List<TemplateDefinition>();
			int kind = this.la.kind;
			FieldDeclaration fd;
			Location startPos;
			string name;
			if (kind <= 100)
			{
				if (kind <= 71)
				{
					if (kind == 2)
					{
						goto IL_D84;
					}
					switch (kind)
					{
					case 47:
					case 49:
					case 50:
					case 51:
						goto IL_D84;
					case 48:
						goto IL_166C;
					default:
						switch (kind)
						{
						case 67:
							break;
						case 68:
						case 69:
							goto IL_166C;
						case 70:
							goto IL_D84;
						case 71:
						{
							m.Check(Modifiers.Fields);
							this.lexer.NextToken();
							m.Add(Modifiers.Const, this.t.Location);
							fd = new FieldDeclaration(attributes, type, m.Modifier);
							fd.StartLocation = m.GetDeclarationLocation(this.t.Location);
							List<VariableDeclaration> constantDeclarators = new List<VariableDeclaration>();
							this.ConstantDeclarator(constantDeclarators);
							while (this.la.kind == 12)
							{
								this.lexer.NextToken();
								this.ConstantDeclarator(constantDeclarators);
							}
							fd.Fields = constantDeclarators;
							fd.EndLocation = this.t.Location;
							base.Expect(1);
							fd.EndLocation = this.t.EndLocation;
							this.compilationUnit.AddChild(fd);
							return;
						}
						default:
							goto IL_166C;
						}
						break;
					}
				}
				else if (kind <= 90)
				{
					switch (kind)
					{
					case 78:
					{
						this.lexer.NextToken();
						m.Check(Modifiers.VBExternalMethods);
						startPos = this.t.Location;
						CharsetModifier charsetModifer = CharsetModifier.None;
						string library = string.Empty;
						string alias = null;
						name = string.Empty;
						if (this.StartOf(15))
						{
							this.Charset(out charsetModifer);
						}
						if (this.la.kind == 167)
						{
							this.lexer.NextToken();
							this.Identifier();
							name = this.t.val;
							base.Expect(115);
							base.Expect(3);
							library = (this.t.literalValue as string);
							if (this.la.kind == 44)
							{
								this.lexer.NextToken();
								base.Expect(3);
								alias = (this.t.literalValue as string);
							}
							if (this.la.kind == 24)
							{
								this.lexer.NextToken();
								if (this.StartOf(4))
								{
									this.FormalParameterList(p);
								}
								base.Expect(25);
							}
							base.Expect(1);
							DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
							declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
							declareDeclaration.EndLocation = this.t.EndLocation;
							this.compilationUnit.AddChild(declareDeclaration);
						}
						else if (this.la.kind == 100)
						{
							this.lexer.NextToken();
							this.Identifier();
							name = this.t.val;
							base.Expect(115);
							base.Expect(3);
							library = (this.t.literalValue as string);
							if (this.la.kind == 44)
							{
								this.lexer.NextToken();
								base.Expect(3);
								alias = (this.t.literalValue as string);
							}
							if (this.la.kind == 24)
							{
								this.lexer.NextToken();
								if (this.StartOf(4))
								{
									this.FormalParameterList(p);
								}
								base.Expect(25);
							}
							if (this.la.kind == 48)
							{
								this.lexer.NextToken();
								this.TypeName(out type);
							}
							base.Expect(1);
							DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
							declareDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
							declareDeclaration.EndLocation = this.t.EndLocation;
							this.compilationUnit.AddChild(declareDeclaration);
						}
						else
						{
							base.SynErr(224);
						}
						return;
					}
					case 79:
						goto IL_166C;
					case 80:
						break;
					default:
						if (kind != 90)
						{
							goto IL_166C;
						}
						break;
					}
				}
				else
				{
					switch (kind)
					{
					case 93:
					{
						this.lexer.NextToken();
						m.Check(Modifiers.VBExternalMethods);
						startPos = this.t.Location;
						name = string.Empty;
						List<InterfaceImplementation> implementsClause = null;
						this.Identifier();
						name = this.t.val;
						if (this.la.kind == 48)
						{
							this.lexer.NextToken();
							this.TypeName(out type);
						}
						else if (this.la.kind == 1 || this.la.kind == 24 || this.la.kind == 107)
						{
							if (this.la.kind == 24)
							{
								this.lexer.NextToken();
								if (this.StartOf(4))
								{
									this.FormalParameterList(p);
								}
								base.Expect(25);
							}
						}
						else
						{
							base.SynErr(225);
						}
						if (this.la.kind == 107)
						{
							this.ImplementsClause(out implementsClause);
						}
						EventDeclaration eventDeclaration = new EventDeclaration(type, m.Modifier, p, attributes, name, implementsClause);
						eventDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
						eventDeclaration.EndLocation = this.t.EndLocation;
						this.compilationUnit.AddChild(eventDeclaration);
						base.Expect(1);
						return;
					}
					case 94:
						goto IL_166C;
					case 95:
						goto IL_D84;
					default:
					{
						if (kind != 100)
						{
							goto IL_166C;
						}
						this.lexer.NextToken();
						m.Check(Modifiers.VBMethods);
						name = string.Empty;
						startPos = this.t.Location;
						List<string> handlesClause = null;
						List<InterfaceImplementation> implementsClause = null;
						AttributeSection returnTypeAttributeSection = null;
						this.Identifier();
						name = this.t.val;
						this.TypeParameterList(templates);
						if (this.la.kind == 24)
						{
							this.lexer.NextToken();
							if (this.StartOf(4))
							{
								this.FormalParameterList(p);
							}
							base.Expect(25);
						}
						if (this.la.kind == 48)
						{
							this.lexer.NextToken();
							while (this.la.kind == 27)
							{
								this.AttributeSection(out returnTypeAttributeSection);
							}
							this.TypeName(out type);
						}
						if (type == null)
						{
							type = new TypeReference("System.Object");
						}
						if (this.la.kind == 105 || this.la.kind == 107)
						{
							if (this.la.kind == 107)
							{
								this.ImplementsClause(out implementsClause);
							}
							else
							{
								this.HandlesClause(out handlesClause);
							}
						}
						base.Expect(1);
						if (Parser.IsMustOverride(m))
						{
							MethodDeclaration methodDeclaration = new MethodDeclaration(name, m.Modifier, type, p, attributes);
							methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
							methodDeclaration.EndLocation = this.t.EndLocation;
							methodDeclaration.HandlesClause = handlesClause;
							methodDeclaration.Templates = templates;
							methodDeclaration.InterfaceImplementations = implementsClause;
							if (returnTypeAttributeSection != null)
							{
								returnTypeAttributeSection.AttributeTarget = "return";
								methodDeclaration.Attributes.Add(returnTypeAttributeSection);
							}
							this.compilationUnit.AddChild(methodDeclaration);
						}
						else if (this.StartOf(14))
						{
							MethodDeclaration methodDeclaration = new MethodDeclaration(name, m.Modifier, type, p, attributes);
							methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
							methodDeclaration.EndLocation = this.t.EndLocation;
							methodDeclaration.Templates = templates;
							methodDeclaration.HandlesClause = handlesClause;
							methodDeclaration.InterfaceImplementations = implementsClause;
							if (returnTypeAttributeSection != null)
							{
								returnTypeAttributeSection.AttributeTarget = "return";
								methodDeclaration.Attributes.Add(returnTypeAttributeSection);
							}
							this.compilationUnit.AddChild(methodDeclaration);
							if (base.ParseMethodBodies)
							{
								this.Block(out stmt);
								base.Expect(88);
								base.Expect(100);
							}
							else
							{
								this.lexer.SkipCurrentBlock(100);
								stmt = new BlockStatement();
							}
							methodDeclaration.Body = (BlockStatement)stmt;
							methodDeclaration.Body.StartLocation = methodDeclaration.EndLocation;
							methodDeclaration.Body.EndLocation = this.t.EndLocation;
							base.Expect(1);
						}
						else
						{
							base.SynErr(223);
						}
						return;
					}
					}
				}
			}
			else if (kind <= 146)
			{
				if (kind <= 121)
				{
					if (kind != 112 && kind != 121)
					{
						goto IL_166C;
					}
				}
				else
				{
					if (kind == 134)
					{
						goto IL_D84;
					}
					switch (kind)
					{
					case 144:
						goto IL_D84;
					case 145:
						goto IL_166C;
					case 146:
					{
						this.lexer.NextToken();
						m.Check(Modifiers.VBProperties);
						startPos = this.t.Location;
						List<InterfaceImplementation> implementsClause = null;
						this.Identifier();
						string propertyName = this.t.val;
						if (this.la.kind == 24)
						{
							this.lexer.NextToken();
							if (this.StartOf(4))
							{
								this.FormalParameterList(p);
							}
							base.Expect(25);
						}
						if (this.la.kind == 48)
						{
							this.lexer.NextToken();
							this.TypeName(out type);
						}
						if (type == null)
						{
							type = new TypeReference("System.Object");
						}
						if (this.la.kind == 107)
						{
							this.ImplementsClause(out implementsClause);
						}
						base.Expect(1);
						if (Parser.IsMustOverride(m))
						{
							PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
							pDecl.StartLocation = m.GetDeclarationLocation(startPos);
							pDecl.EndLocation = this.t.Location;
							pDecl.TypeReference = type;
							pDecl.InterfaceImplementations = implementsClause;
							pDecl.Parameters = p;
							this.compilationUnit.AddChild(pDecl);
						}
						else if (this.StartOf(16))
						{
							PropertyDeclaration pDecl = new PropertyDeclaration(propertyName, type, m.Modifier, attributes);
							pDecl.StartLocation = m.GetDeclarationLocation(startPos);
							pDecl.EndLocation = this.t.Location;
							pDecl.BodyStart = this.t.Location;
							pDecl.TypeReference = type;
							pDecl.InterfaceImplementations = implementsClause;
							pDecl.Parameters = p;
							PropertyGetRegion getRegion;
							PropertySetRegion setRegion;
							this.AccessorDecls(out getRegion, out setRegion);
							base.Expect(88);
							base.Expect(146);
							base.Expect(1);
							pDecl.GetRegion = getRegion;
							pDecl.SetRegion = setRegion;
							pDecl.BodyEnd = this.t.EndLocation;
							this.compilationUnit.AddChild(pDecl);
						}
						else
						{
							base.SynErr(226);
						}
						return;
					}
					default:
						goto IL_166C;
					}
				}
			}
			else
			{
				if (kind > 177)
				{
					if (kind != 187)
					{
						switch (kind)
						{
						case 201:
						case 202:
							break;
						case 203:
							goto IL_166C;
						case 204:
						{
							this.lexer.NextToken();
							startPos = this.t.Location;
							base.Expect(93);
							m.Check(Modifiers.VBExternalMethods);
							EventAddRegion addHandlerAccessorDeclaration = null;
							EventRemoveRegion removeHandlerAccessorDeclaration = null;
							EventRaiseRegion raiseEventAccessorDeclaration = null;
							List<InterfaceImplementation> implementsClause = null;
							this.Identifier();
							string customEventName = this.t.val;
							base.Expect(48);
							this.TypeName(out type);
							if (this.la.kind == 107)
							{
								this.ImplementsClause(out implementsClause);
							}
							base.Expect(1);
							while (this.StartOf(17))
							{
								EventAddRemoveRegion eventAccessorDeclaration;
								this.EventAccessorDeclaration(out eventAccessorDeclaration);
								if (eventAccessorDeclaration is EventAddRegion)
								{
									addHandlerAccessorDeclaration = (EventAddRegion)eventAccessorDeclaration;
								}
								else if (eventAccessorDeclaration is EventRemoveRegion)
								{
									removeHandlerAccessorDeclaration = (EventRemoveRegion)eventAccessorDeclaration;
								}
								else if (eventAccessorDeclaration is EventRaiseRegion)
								{
									raiseEventAccessorDeclaration = (EventRaiseRegion)eventAccessorDeclaration;
								}
							}
							base.Expect(88);
							base.Expect(93);
							base.Expect(1);
							if (addHandlerAccessorDeclaration == null)
							{
								this.Error("Need to provide AddHandler accessor.");
							}
							if (removeHandlerAccessorDeclaration == null)
							{
								this.Error("Need to provide RemoveHandler accessor.");
							}
							if (raiseEventAccessorDeclaration == null)
							{
								this.Error("Need to provide RaiseEvent accessor.");
							}
							EventDeclaration decl = new EventDeclaration(type, customEventName, m.Modifier, attributes, null);
							decl.StartLocation = m.GetDeclarationLocation(startPos);
							decl.EndLocation = this.t.EndLocation;
							decl.AddRegion = addHandlerAccessorDeclaration;
							decl.RemoveRegion = removeHandlerAccessorDeclaration;
							decl.RaiseRegion = raiseEventAccessorDeclaration;
							this.compilationUnit.AddChild(decl);
							return;
						}
						default:
							goto IL_166C;
						}
					}
					ConversionType opConversionType = ConversionType.None;
					if (this.la.kind == 201 || this.la.kind == 202)
					{
						if (this.la.kind == 202)
						{
							this.lexer.NextToken();
							opConversionType = ConversionType.Implicit;
						}
						else
						{
							this.lexer.NextToken();
							opConversionType = ConversionType.Explicit;
						}
					}
					base.Expect(187);
					m.Check(Modifiers.VBOperators);
					startPos = this.t.Location;
					TypeReference returnType = NullTypeReference.Instance;
					TypeReference operandType = NullTypeReference.Instance;
					List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
					List<AttributeSection> returnTypeAttributes = new List<AttributeSection>();
					OverloadableOperatorType operatorType;
					this.OverloadableOperator(out operatorType);
					base.Expect(24);
					if (this.la.kind == 55)
					{
						this.lexer.NextToken();
					}
					this.Identifier();
					string operandName = this.t.val;
					if (this.la.kind == 48)
					{
						this.lexer.NextToken();
						this.TypeName(out operandType);
					}
					parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In));
					while (this.la.kind == 12)
					{
						this.lexer.NextToken();
						if (this.la.kind == 55)
						{
							this.lexer.NextToken();
						}
						this.Identifier();
						operandName = this.t.val;
						if (this.la.kind == 48)
						{
							this.lexer.NextToken();
							this.TypeName(out operandType);
						}
						parameters.Add(new ParameterDeclarationExpression(operandType, operandName, ParameterModifiers.In));
					}
					base.Expect(25);
					Location endPos = this.t.EndLocation;
					if (this.la.kind == 48)
					{
						this.lexer.NextToken();
						while (this.la.kind == 27)
						{
							AttributeSection section;
							this.AttributeSection(out section);
							returnTypeAttributes.Add(section);
						}
						this.TypeName(out returnType);
						endPos = this.t.EndLocation;
						base.Expect(1);
					}
					this.Block(out stmt);
					base.Expect(88);
					base.Expect(187);
					base.Expect(1);
					OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier, attributes, parameters, returnType, operatorType);
					operatorDeclaration.ConversionType = opConversionType;
					operatorDeclaration.ReturnTypeAttributes = returnTypeAttributes;
					operatorDeclaration.Body = (BlockStatement)stmt;
					operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
					operatorDeclaration.EndLocation = endPos;
					operatorDeclaration.Body.StartLocation = startPos;
					operatorDeclaration.Body.EndLocation = this.t.Location;
					this.compilationUnit.AddChild(operatorDeclaration);
					return;
				}
				switch (kind)
				{
				case 166:
					break;
				case 167:
					this.lexer.NextToken();
					startPos = this.t.Location;
					if (this.StartOf(13))
					{
						name = string.Empty;
						List<string> handlesClause = null;
						List<InterfaceImplementation> implementsClause = null;
						this.Identifier();
						name = this.t.val;
						m.Check(Modifiers.VBMethods);
						this.TypeParameterList(templates);
						if (this.la.kind == 24)
						{
							this.lexer.NextToken();
							if (this.StartOf(4))
							{
								this.FormalParameterList(p);
							}
							base.Expect(25);
						}
						if (this.la.kind == 105 || this.la.kind == 107)
						{
							if (this.la.kind == 107)
							{
								this.ImplementsClause(out implementsClause);
							}
							else
							{
								this.HandlesClause(out handlesClause);
							}
						}
						Location endLocation = this.t.EndLocation;
						base.Expect(1);
						if (Parser.IsMustOverride(m))
						{
							MethodDeclaration methodDeclaration = new MethodDeclaration(name, m.Modifier, null, p, attributes);
							methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
							methodDeclaration.EndLocation = endLocation;
							methodDeclaration.TypeReference = new TypeReference("", "System.Void");
							methodDeclaration.Templates = templates;
							methodDeclaration.HandlesClause = handlesClause;
							methodDeclaration.InterfaceImplementations = implementsClause;
							this.compilationUnit.AddChild(methodDeclaration);
						}
						else if (this.StartOf(14))
						{
							MethodDeclaration methodDeclaration = new MethodDeclaration(name, m.Modifier, null, p, attributes);
							methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
							methodDeclaration.EndLocation = endLocation;
							methodDeclaration.TypeReference = new TypeReference("", "System.Void");
							methodDeclaration.Templates = templates;
							methodDeclaration.HandlesClause = handlesClause;
							methodDeclaration.InterfaceImplementations = implementsClause;
							this.compilationUnit.AddChild(methodDeclaration);
							if (base.ParseMethodBodies)
							{
								this.Block(out stmt);
								base.Expect(88);
								base.Expect(167);
							}
							else
							{
								this.lexer.SkipCurrentBlock(167);
								stmt = new BlockStatement();
							}
							methodDeclaration.Body = (BlockStatement)stmt;
							methodDeclaration.Body.EndLocation = this.t.EndLocation;
							base.Expect(1);
						}
						else
						{
							base.SynErr(221);
						}
					}
					else if (this.la.kind == 127)
					{
						this.lexer.NextToken();
						if (this.la.kind == 24)
						{
							this.lexer.NextToken();
							if (this.StartOf(4))
							{
								this.FormalParameterList(p);
							}
							base.Expect(25);
						}
						m.Check(Modifiers.Constructors);
						Location constructorEndLocation = this.t.EndLocation;
						base.Expect(1);
						if (base.ParseMethodBodies)
						{
							this.Block(out stmt);
							base.Expect(88);
							base.Expect(167);
						}
						else
						{
							this.lexer.SkipCurrentBlock(167);
							stmt = new BlockStatement();
						}
						Location endLocation = this.t.EndLocation;
						base.Expect(1);
						ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes);
						cd.StartLocation = m.GetDeclarationLocation(startPos);
						cd.EndLocation = constructorEndLocation;
						cd.Body = (BlockStatement)stmt;
						cd.Body.EndLocation = endLocation;
						this.compilationUnit.AddChild(cd);
					}
					else
					{
						base.SynErr(222);
					}
					return;
				case 168:
					goto IL_166C;
				case 169:
					goto IL_D84;
				default:
					switch (kind)
					{
					case 176:
					case 177:
						goto IL_D84;
					default:
						goto IL_166C;
					}
					break;
				}
			}
			this.NonModuleDeclaration(m, attributes);
			return;
			IL_D84:
			startPos = this.t.Location;
			m.Check(Modifiers.Fields);
			fd = new FieldDeclaration(attributes, null, m.Modifier);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			this.IdentifierForFieldDeclaration();
			name = this.t.val;
			this.VariableDeclaratorPartAfterIdentifier(variableDeclarators, name);
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.VariableDeclarator(variableDeclarators);
			}
			base.Expect(1);
			fd.EndLocation = this.t.EndLocation;
			fd.Fields = variableDeclarators;
			this.compilationUnit.AddChild(fd);
			return;
			IL_166C:
			base.SynErr(227);
		}

		private void EnumMemberDecl(out FieldDeclaration f)
		{
			Expression expr = null;
			List<AttributeSection> attributes = new List<AttributeSection>();
			AttributeSection section = null;
			while (this.la.kind == 27)
			{
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			this.Identifier();
			f = new FieldDeclaration(attributes);
			VariableDeclaration varDecl = new VariableDeclaration(this.t.val);
			f.Fields.Add(varDecl);
			f.StartLocation = this.t.Location;
			if (this.la.kind == 11)
			{
				this.lexer.NextToken();
				this.Expr(out expr);
				varDecl.Initializer = expr;
			}
			base.Expect(1);
		}

		private void InterfaceMemberDecl()
		{
			TypeReference type = null;
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			List<TemplateDefinition> templates = new List<TemplateDefinition>();
			AttributeSection returnTypeAttributeSection = null;
			ModifierList mod = new ModifierList();
			List<AttributeSection> attributes = new List<AttributeSection>();
			if (this.StartOf(18))
			{
				while (this.la.kind == 27)
				{
					AttributeSection section;
					this.AttributeSection(out section);
					attributes.Add(section);
				}
				while (this.StartOf(8))
				{
					this.MemberModifier(mod);
				}
				if (this.la.kind == 93)
				{
					this.lexer.NextToken();
					mod.Check(Modifiers.New);
					Location startLocation = this.t.Location;
					this.Identifier();
					string name = this.t.val;
					if (this.la.kind == 24)
					{
						this.lexer.NextToken();
						if (this.StartOf(4))
						{
							this.FormalParameterList(p);
						}
						base.Expect(25);
					}
					if (this.la.kind == 48)
					{
						this.lexer.NextToken();
						this.TypeName(out type);
					}
					base.Expect(1);
					EventDeclaration ed = new EventDeclaration(type, mod.Modifier, p, attributes, name, null);
					this.compilationUnit.AddChild(ed);
					ed.StartLocation = startLocation;
					ed.EndLocation = this.t.EndLocation;
				}
				else if (this.la.kind == 167)
				{
					this.lexer.NextToken();
					Location startLocation = this.t.Location;
					mod.Check(Modifiers.VBInterfaceMethods);
					this.Identifier();
					string name = this.t.val;
					this.TypeParameterList(templates);
					if (this.la.kind == 24)
					{
						this.lexer.NextToken();
						if (this.StartOf(4))
						{
							this.FormalParameterList(p);
						}
						base.Expect(25);
					}
					base.Expect(1);
					MethodDeclaration md = new MethodDeclaration(name, mod.Modifier, null, p, attributes);
					md.TypeReference = new TypeReference("", "System.Void");
					md.StartLocation = startLocation;
					md.EndLocation = this.t.EndLocation;
					md.Templates = templates;
					this.compilationUnit.AddChild(md);
				}
				else if (this.la.kind == 100)
				{
					this.lexer.NextToken();
					mod.Check(Modifiers.VBInterfaceMethods);
					Location startLocation = this.t.Location;
					this.Identifier();
					string name = this.t.val;
					this.TypeParameterList(templates);
					if (this.la.kind == 24)
					{
						this.lexer.NextToken();
						if (this.StartOf(4))
						{
							this.FormalParameterList(p);
						}
						base.Expect(25);
					}
					if (this.la.kind == 48)
					{
						this.lexer.NextToken();
						while (this.la.kind == 27)
						{
							this.AttributeSection(out returnTypeAttributeSection);
						}
						this.TypeName(out type);
					}
					if (type == null)
					{
						type = new TypeReference("System.Object");
					}
					MethodDeclaration md = new MethodDeclaration(name, mod.Modifier, type, p, attributes);
					if (returnTypeAttributeSection != null)
					{
						returnTypeAttributeSection.AttributeTarget = "return";
						md.Attributes.Add(returnTypeAttributeSection);
					}
					md.StartLocation = startLocation;
					md.EndLocation = this.t.EndLocation;
					md.Templates = templates;
					this.compilationUnit.AddChild(md);
					base.Expect(1);
				}
				else if (this.la.kind == 146)
				{
					this.lexer.NextToken();
					Location startLocation = this.t.Location;
					mod.Check(Modifiers.VBInterfaceProperties);
					this.Identifier();
					string name = this.t.val;
					if (this.la.kind == 24)
					{
						this.lexer.NextToken();
						if (this.StartOf(4))
						{
							this.FormalParameterList(p);
						}
						base.Expect(25);
					}
					if (this.la.kind == 48)
					{
						this.lexer.NextToken();
						this.TypeName(out type);
					}
					if (type == null)
					{
						type = new TypeReference("System.Object");
					}
					base.Expect(1);
					PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
					pd.Parameters = p;
					pd.EndLocation = this.t.EndLocation;
					pd.StartLocation = startLocation;
					this.compilationUnit.AddChild(pd);
				}
				else
				{
					base.SynErr(228);
				}
			}
			else if (this.StartOf(19))
			{
				this.NonModuleDeclaration(mod, attributes);
			}
			else
			{
				base.SynErr(229);
			}
		}

		private void Expr(out Expression expr)
		{
			this.DisjunctionExpr(out expr);
		}

		private void ImplementsClause(out List<InterfaceImplementation> baseInterfaces)
		{
			baseInterfaces = new List<InterfaceImplementation>();
			TypeReference type = null;
			string memberName = null;
			base.Expect(107);
			this.NonArrayTypeName(out type, false);
			if (type != null)
			{
				memberName = TypeReference.StripLastIdentifierFromType(ref type);
			}
			baseInterfaces.Add(new InterfaceImplementation(type, memberName));
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.NonArrayTypeName(out type, false);
				if (type != null)
				{
					memberName = TypeReference.StripLastIdentifierFromType(ref type);
				}
				baseInterfaces.Add(new InterfaceImplementation(type, memberName));
			}
		}

		private void HandlesClause(out List<string> handlesClause)
		{
			handlesClause = new List<string>();
			base.Expect(105);
			string name;
			this.EventMemberSpecifier(out name);
			handlesClause.Add(name);
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.EventMemberSpecifier(out name);
				handlesClause.Add(name);
			}
		}

		private void Block(out Statement stmt)
		{
			BlockStatement blockStmt = new BlockStatement();
			if (this.t != null)
			{
				blockStmt.StartLocation = this.t.Location;
			}
			this.compilationUnit.BlockStart(blockStmt);
			while (this.StartOf(20) || this.IsEndStmtAhead())
			{
				if (this.IsEndStmtAhead())
				{
					base.Expect(88);
					this.EndOfStmt();
					this.compilationUnit.AddChild(new EndStatement());
				}
				else
				{
					this.Statement();
					this.EndOfStmt();
				}
			}
			stmt = blockStmt;
			if (this.t != null)
			{
				blockStmt.EndLocation = this.t.EndLocation;
			}
			this.compilationUnit.BlockEnd();
		}

		private void Charset(out CharsetModifier charsetModifier)
		{
			charsetModifier = CharsetModifier.None;
			if (this.la.kind != 100 && this.la.kind != 167)
			{
				if (this.la.kind == 47)
				{
					this.lexer.NextToken();
					charsetModifier = CharsetModifier.Ansi;
				}
				else if (this.la.kind == 50)
				{
					this.lexer.NextToken();
					charsetModifier = CharsetModifier.Auto;
				}
				else if (this.la.kind == 176)
				{
					this.lexer.NextToken();
					charsetModifier = CharsetModifier.Unicode;
				}
				else
				{
					base.SynErr(230);
				}
			}
		}

		private void IdentifierForFieldDeclaration()
		{
			int kind = this.la.kind;
			if (kind <= 95)
			{
				if (kind <= 51)
				{
					if (kind == 2)
					{
						this.lexer.NextToken();
						return;
					}
					switch (kind)
					{
					case 47:
						this.lexer.NextToken();
						return;
					case 49:
						this.lexer.NextToken();
						return;
					case 50:
						this.lexer.NextToken();
						return;
					case 51:
						this.lexer.NextToken();
						return;
					}
				}
				else
				{
					if (kind == 70)
					{
						this.lexer.NextToken();
						return;
					}
					if (kind == 95)
					{
						this.lexer.NextToken();
						return;
					}
				}
			}
			else if (kind <= 144)
			{
				if (kind == 134)
				{
					this.lexer.NextToken();
					return;
				}
				if (kind == 144)
				{
					this.lexer.NextToken();
					return;
				}
			}
			else
			{
				if (kind == 169)
				{
					this.lexer.NextToken();
					return;
				}
				switch (kind)
				{
				case 176:
					this.lexer.NextToken();
					return;
				case 177:
					this.lexer.NextToken();
					return;
				}
			}
			base.SynErr(231);
		}

		private void VariableDeclaratorPartAfterIdentifier(List<VariableDeclaration> fieldDeclaration, string name)
		{
			Expression expr = null;
			TypeReference type = null;
			ArrayList rank = null;
			List<Expression> dimension = null;
			if (this.IsSize() && !this.IsDims())
			{
				this.ArrayInitializationModifier(out dimension);
			}
			if (this.IsDims())
			{
				this.ArrayNameModifier(out rank);
			}
			if (this.IsObjectCreation())
			{
				base.Expect(48);
				this.ObjectCreateExpression(out expr);
				if (expr is ObjectCreateExpression)
				{
					type = ((ObjectCreateExpression)expr).CreateType;
				}
				else
				{
					type = ((ArrayCreateExpression)expr).CreateType;
				}
			}
			else if (this.StartOf(21))
			{
				if (this.la.kind == 48)
				{
					this.lexer.NextToken();
					this.TypeName(out type);
					if (type != null)
					{
						for (int i = fieldDeclaration.Count - 1; i >= 0; i--)
						{
							VariableDeclaration vd = fieldDeclaration[i];
							if (vd.TypeReference.Type.Length > 0)
							{
								break;
							}
							TypeReference newType = type.Clone();
							newType.RankSpecifier = vd.TypeReference.RankSpecifier;
							vd.TypeReference = newType;
						}
					}
				}
				if (type == null && (dimension != null || rank != null))
				{
					type = new TypeReference("");
				}
				if (dimension != null)
				{
					if (type.RankSpecifier != null)
					{
						this.Error("array rank only allowed one time");
					}
					else
					{
						if (rank == null)
						{
							type.RankSpecifier = new int[]
							{
								dimension.Count - 1
							};
						}
						else
						{
							rank.Insert(0, dimension.Count - 1);
							type.RankSpecifier = (int[])rank.ToArray(typeof(int));
						}
						expr = new ArrayCreateExpression(type, dimension);
					}
				}
				else if (rank != null)
				{
					if (type.RankSpecifier != null)
					{
						this.Error("array rank only allowed one time");
					}
					else
					{
						type.RankSpecifier = (int[])rank.ToArray(typeof(int));
					}
				}
				if (this.la.kind == 11)
				{
					this.lexer.NextToken();
					this.VariableInitializer(out expr);
				}
			}
			else
			{
				base.SynErr(232);
			}
			fieldDeclaration.Add(new VariableDeclaration(name, expr, type));
		}

		private void VariableDeclarator(List<VariableDeclaration> fieldDeclaration)
		{
			this.Identifier();
			string name = this.t.val;
			this.VariableDeclaratorPartAfterIdentifier(fieldDeclaration, name);
		}

		private void ConstantDeclarator(List<VariableDeclaration> constantDeclaration)
		{
			Expression expr = null;
			TypeReference type = null;
			string name = string.Empty;
			this.Identifier();
			name = this.t.val;
			if (this.la.kind == 48)
			{
				this.lexer.NextToken();
				this.TypeName(out type);
			}
			base.Expect(11);
			this.Expr(out expr);
			constantDeclaration.Add(new VariableDeclaration(name, expr)
			{
				TypeReference = type
			});
		}

		private void AccessorDecls(out PropertyGetRegion getBlock, out PropertySetRegion setBlock)
		{
			List<AttributeSection> attributes = new List<AttributeSection>();
			getBlock = null;
			setBlock = null;
			while (this.la.kind == 27)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			if (this.StartOf(22))
			{
				this.GetAccessorDecl(out getBlock, attributes);
				if (this.StartOf(23))
				{
					attributes = new List<AttributeSection>();
					while (this.la.kind == 27)
					{
						AttributeSection section;
						this.AttributeSection(out section);
						attributes.Add(section);
					}
					this.SetAccessorDecl(out setBlock, attributes);
				}
			}
			else if (this.StartOf(24))
			{
				this.SetAccessorDecl(out setBlock, attributes);
				if (this.StartOf(25))
				{
					attributes = new List<AttributeSection>();
					while (this.la.kind == 27)
					{
						AttributeSection section;
						this.AttributeSection(out section);
						attributes.Add(section);
					}
					this.GetAccessorDecl(out getBlock, attributes);
				}
			}
			else
			{
				base.SynErr(233);
			}
		}

		private void EventAccessorDeclaration(out EventAddRemoveRegion eventAccessorDeclaration)
		{
			Statement stmt = null;
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			List<AttributeSection> attributes = new List<AttributeSection>();
			eventAccessorDeclaration = null;
			while (this.la.kind == 27)
			{
				AttributeSection section;
				this.AttributeSection(out section);
				attributes.Add(section);
			}
			if (this.la.kind == 42)
			{
				this.lexer.NextToken();
				if (this.la.kind == 24)
				{
					this.lexer.NextToken();
					if (this.StartOf(4))
					{
						this.FormalParameterList(p);
					}
					base.Expect(25);
				}
				base.Expect(1);
				this.Block(out stmt);
				base.Expect(88);
				base.Expect(42);
				base.Expect(1);
				eventAccessorDeclaration = new EventAddRegion(attributes);
				eventAccessorDeclaration.Block = (BlockStatement)stmt;
				eventAccessorDeclaration.Parameters = p;
			}
			else if (this.la.kind == 152)
			{
				this.lexer.NextToken();
				if (this.la.kind == 24)
				{
					this.lexer.NextToken();
					if (this.StartOf(4))
					{
						this.FormalParameterList(p);
					}
					base.Expect(25);
				}
				base.Expect(1);
				this.Block(out stmt);
				base.Expect(88);
				base.Expect(152);
				base.Expect(1);
				eventAccessorDeclaration = new EventRemoveRegion(attributes);
				eventAccessorDeclaration.Block = (BlockStatement)stmt;
				eventAccessorDeclaration.Parameters = p;
			}
			else if (this.la.kind == 149)
			{
				this.lexer.NextToken();
				if (this.la.kind == 24)
				{
					this.lexer.NextToken();
					if (this.StartOf(4))
					{
						this.FormalParameterList(p);
					}
					base.Expect(25);
				}
				base.Expect(1);
				this.Block(out stmt);
				base.Expect(88);
				base.Expect(149);
				base.Expect(1);
				eventAccessorDeclaration = new EventRaiseRegion(attributes);
				eventAccessorDeclaration.Block = (BlockStatement)stmt;
				eventAccessorDeclaration.Parameters = p;
			}
			else
			{
				base.SynErr(234);
			}
		}

		private void OverloadableOperator(out OverloadableOperatorType operatorType)
		{
			operatorType = OverloadableOperatorType.None;
			int kind = this.la.kind;
			if (kind <= 120)
			{
				if (kind <= 70)
				{
					switch (kind)
					{
					case 2:
						break;
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 12:
					case 13:
					case 21:
					case 22:
					case 23:
					case 24:
					case 25:
						goto IL_396;
					case 11:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.Equality;
						return;
					case 14:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.Add;
						return;
					case 15:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.Subtract;
						return;
					case 16:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.Multiply;
						return;
					case 17:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.Divide;
						return;
					case 18:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.DivideInteger;
						return;
					case 19:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.Concat;
						return;
					case 20:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.Power;
						return;
					case 26:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.GreaterThan;
						return;
					case 27:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.LessThan;
						return;
					case 28:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.InEquality;
						return;
					case 29:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.GreaterThanOrEqual;
						return;
					case 30:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.LessThanOrEqual;
						return;
					case 31:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.ShiftLeft;
						return;
					case 32:
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.ShiftRight;
						return;
					default:
						switch (kind)
						{
						case 45:
							this.lexer.NextToken();
							operatorType = OverloadableOperatorType.BitwiseAnd;
							return;
						case 46:
						case 48:
							goto IL_396;
						case 47:
						case 49:
						case 50:
						case 51:
							break;
						default:
							if (kind != 70)
							{
								goto IL_396;
							}
							break;
						}
						break;
					}
				}
				else if (kind <= 95)
				{
					if (kind == 75)
					{
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.CType;
						return;
					}
					if (kind != 95)
					{
						goto IL_396;
					}
				}
				else
				{
					if (kind == 116)
					{
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.Like;
						return;
					}
					if (kind != 120)
					{
						goto IL_396;
					}
					this.lexer.NextToken();
					operatorType = OverloadableOperatorType.Modulus;
					return;
				}
			}
			else if (kind <= 144)
			{
				if (kind != 134)
				{
					if (kind == 138)
					{
						this.lexer.NextToken();
						operatorType = OverloadableOperatorType.BitwiseOr;
						return;
					}
					if (kind != 144)
					{
						goto IL_396;
					}
				}
			}
			else if (kind <= 177)
			{
				if (kind != 169)
				{
					switch (kind)
					{
					case 176:
					case 177:
						break;
					default:
						goto IL_396;
					}
				}
			}
			else
			{
				if (kind == 185)
				{
					this.lexer.NextToken();
					operatorType = OverloadableOperatorType.ExclusiveOr;
					return;
				}
				if (kind != 204)
				{
					goto IL_396;
				}
			}
			this.Identifier();
			string opName = this.t.val;
			if (string.Equals(opName, "istrue", StringComparison.InvariantCultureIgnoreCase))
			{
				operatorType = OverloadableOperatorType.IsTrue;
			}
			else if (string.Equals(opName, "isfalse", StringComparison.InvariantCultureIgnoreCase))
			{
				operatorType = OverloadableOperatorType.IsFalse;
			}
			else
			{
				this.Error("Invalid operator. Possible operators are '+', '-', 'Not', 'IsTrue', 'IsFalse'.");
			}
			return;
			IL_396:
			base.SynErr(235);
		}

		private void GetAccessorDecl(out PropertyGetRegion getBlock, List<AttributeSection> attributes)
		{
			Statement stmt = null;
			Modifiers i;
			this.PropertyAccessorAccessModifier(out i);
			base.Expect(101);
			Location startLocation = this.t.Location;
			base.Expect(1);
			this.Block(out stmt);
			getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes);
			base.Expect(88);
			base.Expect(101);
			getBlock.Modifier = i;
			getBlock.StartLocation = startLocation;
			getBlock.EndLocation = this.t.EndLocation;
			base.Expect(1);
		}

		private void SetAccessorDecl(out PropertySetRegion setBlock, List<AttributeSection> attributes)
		{
			Statement stmt = null;
			List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
			Modifiers i;
			this.PropertyAccessorAccessModifier(out i);
			base.Expect(156);
			Location startLocation = this.t.Location;
			if (this.la.kind == 24)
			{
				this.lexer.NextToken();
				if (this.StartOf(4))
				{
					this.FormalParameterList(p);
				}
				base.Expect(25);
			}
			base.Expect(1);
			this.Block(out stmt);
			setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
			setBlock.Modifier = i;
			setBlock.Parameters = p;
			base.Expect(88);
			base.Expect(156);
			setBlock.StartLocation = startLocation;
			setBlock.EndLocation = this.t.EndLocation;
			base.Expect(1);
		}

		private void PropertyAccessorAccessModifier(out Modifiers m)
		{
			m = Modifiers.None;
			while (this.StartOf(26))
			{
				if (this.la.kind == 148)
				{
					this.lexer.NextToken();
					m |= Modifiers.Public;
				}
				else if (this.la.kind == 147)
				{
					this.lexer.NextToken();
					m |= Modifiers.Protected;
				}
				else if (this.la.kind == 99)
				{
					this.lexer.NextToken();
					m |= Modifiers.Internal;
				}
				else
				{
					this.lexer.NextToken();
					m |= Modifiers.Private;
				}
			}
		}

		private void ArrayInitializationModifier(out List<Expression> arrayModifiers)
		{
			arrayModifiers = null;
			base.Expect(24);
			this.InitializationRankList(out arrayModifiers);
			base.Expect(25);
		}

		private void ArrayNameModifier(out ArrayList arrayModifiers)
		{
			arrayModifiers = null;
			this.ArrayTypeModifiers(out arrayModifiers);
		}

		private void ObjectCreateExpression(out Expression oce)
		{
			TypeReference type = null;
			Expression initializer = null;
			List<Expression> arguments = null;
			ArrayList dimensions = null;
			oce = null;
			base.Expect(127);
			this.NonArrayTypeName(out type, false);
			if (this.la.kind == 24)
			{
				this.lexer.NextToken();
				bool canBeNormal;
				bool canBeReDim;
				this.NormalOrReDimArgumentList(out arguments, out canBeNormal, out canBeReDim);
				base.Expect(25);
				if (this.la.kind == 22 || this.la.kind == 24)
				{
					if (this.la.kind == 24)
					{
						this.ArrayTypeModifiers(out dimensions);
						this.ArrayInitializer(out initializer);
					}
					else
					{
						this.ArrayInitializer(out initializer);
					}
				}
				if (canBeReDim && !canBeNormal && initializer == null)
				{
					initializer = new ArrayInitializerExpression();
				}
			}
			if (type == null)
			{
				type = new TypeReference("Object");
			}
			if (initializer == null)
			{
				oce = new ObjectCreateExpression(type, arguments);
			}
			else
			{
				if (dimensions == null)
				{
					dimensions = new ArrayList();
				}
				dimensions.Insert(0, (arguments == null) ? 0 : Math.Max(arguments.Count - 1, 0));
				type.RankSpecifier = (int[])dimensions.ToArray(typeof(int));
				oce = new ArrayCreateExpression(type, initializer as ArrayInitializerExpression)
				{
					Arguments = arguments
				};
			}
		}

		private void VariableInitializer(out Expression initializerExpression)
		{
			initializerExpression = null;
			if (this.StartOf(27))
			{
				this.Expr(out initializerExpression);
			}
			else if (this.la.kind == 22)
			{
				this.ArrayInitializer(out initializerExpression);
			}
			else
			{
				base.SynErr(236);
			}
		}

		private void InitializationRankList(out List<Expression> rank)
		{
			rank = new List<Expression>();
			Expression expr = null;
			this.Expr(out expr);
			if (this.la.kind == 172)
			{
				this.lexer.NextToken();
				this.EnsureIsZero(expr);
				this.Expr(out expr);
			}
			if (expr != null)
			{
				rank.Add(expr);
			}
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.Expr(out expr);
				if (this.la.kind == 172)
				{
					this.lexer.NextToken();
					this.EnsureIsZero(expr);
					this.Expr(out expr);
				}
				if (expr != null)
				{
					rank.Add(expr);
				}
			}
		}

		private void ArrayInitializer(out Expression outExpr)
		{
			Expression expr = null;
			ArrayInitializerExpression initializer = new ArrayInitializerExpression();
			base.Expect(22);
			if (this.StartOf(28))
			{
				this.VariableInitializer(out expr);
				if (expr != null)
				{
					initializer.CreateExpressions.Add(expr);
				}
				while (this.NotFinalComma())
				{
					base.Expect(12);
					this.VariableInitializer(out expr);
					if (expr != null)
					{
						initializer.CreateExpressions.Add(expr);
					}
				}
			}
			base.Expect(23);
			outExpr = initializer;
		}

		private void EventMemberSpecifier(out string name)
		{
			if (this.StartOf(13))
			{
				this.Identifier();
			}
			else if (this.la.kind == 124)
			{
				this.lexer.NextToken();
			}
			else if (this.la.kind == 119)
			{
				this.lexer.NextToken();
			}
			else
			{
				base.SynErr(237);
			}
			name = this.t.val;
			base.Expect(10);
			string eventName;
			this.IdentifierOrKeyword(out eventName);
			name = name + "." + eventName;
		}

		private void IdentifierOrKeyword(out string name)
		{
			this.lexer.NextToken();
			name = this.t.val;
		}

		private void DisjunctionExpr(out Expression outExpr)
		{
			this.ConjunctionExpr(out outExpr);
			while (this.la.kind == 138 || this.la.kind == 139 || this.la.kind == 185)
			{
				BinaryOperatorType op;
				if (this.la.kind == 138)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.BitwiseOr;
				}
				else if (this.la.kind == 139)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.LogicalOr;
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.ExclusiveOr;
				}
				Expression expr;
				this.ConjunctionExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void AssignmentOperator(out AssignmentOperatorType op)
		{
			op = AssignmentOperatorType.None;
			int kind = this.la.kind;
			if (kind != 11)
			{
				switch (kind)
				{
				case 33:
					this.lexer.NextToken();
					op = AssignmentOperatorType.Add;
					break;
				case 34:
					this.lexer.NextToken();
					op = AssignmentOperatorType.Power;
					break;
				case 35:
					this.lexer.NextToken();
					op = AssignmentOperatorType.Subtract;
					break;
				case 36:
					this.lexer.NextToken();
					op = AssignmentOperatorType.Multiply;
					break;
				case 37:
					this.lexer.NextToken();
					op = AssignmentOperatorType.Divide;
					break;
				case 38:
					this.lexer.NextToken();
					op = AssignmentOperatorType.DivideInteger;
					break;
				case 39:
					this.lexer.NextToken();
					op = AssignmentOperatorType.ShiftLeft;
					break;
				case 40:
					this.lexer.NextToken();
					op = AssignmentOperatorType.ShiftRight;
					break;
				case 41:
					this.lexer.NextToken();
					op = AssignmentOperatorType.ConcatString;
					break;
				default:
					base.SynErr(238);
					break;
				}
			}
			else
			{
				this.lexer.NextToken();
				op = AssignmentOperatorType.Assign;
			}
		}

		private void SimpleExpr(out Expression pexpr)
		{
			this.SimpleNonInvocationExpression(out pexpr);
			while (this.la.kind == 10 || this.la.kind == 24)
			{
				if (this.la.kind == 10)
				{
					this.lexer.NextToken();
					string name;
					this.IdentifierOrKeyword(out name);
					pexpr = new FieldReferenceExpression(pexpr, name);
				}
				else
				{
					this.InvocationExpression(ref pexpr);
				}
			}
		}

		private void SimpleNonInvocationExpression(out Expression pexpr)
		{
			TypeReference type = null;
			string name = string.Empty;
			pexpr = null;
			if (this.StartOf(29))
			{
				int kind = this.la.kind;
				Expression expr;
				if (kind <= 119)
				{
					if (kind <= 96)
					{
						switch (kind)
						{
						case 2:
							break;
						case 3:
							this.lexer.NextToken();
							pexpr = new PrimitiveExpression(this.t.literalValue, this.t.val);
							goto IL_79D;
						case 4:
							this.lexer.NextToken();
							pexpr = new PrimitiveExpression(this.t.literalValue, this.t.val);
							goto IL_79D;
						case 5:
							this.lexer.NextToken();
							pexpr = new PrimitiveExpression(this.t.literalValue, this.t.val);
							goto IL_79D;
						case 6:
							this.lexer.NextToken();
							pexpr = new PrimitiveExpression(this.t.literalValue, this.t.val);
							goto IL_79D;
						case 7:
							this.lexer.NextToken();
							pexpr = new PrimitiveExpression(this.t.literalValue, this.t.val);
							goto IL_79D;
						case 8:
							this.lexer.NextToken();
							pexpr = new PrimitiveExpression(this.t.literalValue, this.t.val);
							goto IL_79D;
						case 9:
							this.lexer.NextToken();
							pexpr = new PrimitiveExpression(this.t.literalValue, this.t.val);
							goto IL_79D;
						default:
							if (kind == 24)
							{
								this.lexer.NextToken();
								this.Expr(out expr);
								base.Expect(25);
								pexpr = new ParenthesizedExpression(expr);
								goto IL_79D;
							}
							switch (kind)
							{
							case 43:
								this.lexer.NextToken();
								this.Expr(out expr);
								pexpr = new AddressOfExpression(expr);
								goto IL_79D;
							case 44:
							case 45:
							case 46:
							case 48:
							case 53:
							case 55:
							case 56:
							case 57:
							case 58:
							case 67:
							case 71:
							case 78:
							case 79:
							case 80:
							case 81:
							case 83:
							case 85:
							case 86:
							case 87:
							case 88:
							case 89:
							case 90:
							case 91:
							case 92:
							case 93:
							case 94:
								goto IL_79D;
							case 47:
							case 49:
							case 50:
							case 51:
							case 70:
							case 95:
								break;
							case 52:
							case 54:
							case 65:
							case 76:
							case 77:
							case 84:
								goto IL_471;
							case 59:
							case 60:
							case 61:
							case 62:
							case 63:
							case 64:
							case 66:
							case 68:
							case 69:
							case 72:
							case 73:
							case 74:
								goto IL_6E4;
							case 75:
							case 82:
								goto IL_616;
							case 96:
								this.lexer.NextToken();
								pexpr = new PrimitiveExpression(false, "false");
								goto IL_79D;
							default:
								goto IL_79D;
							}
							break;
						}
					}
					else
					{
						if (kind == 102)
						{
							this.lexer.NextToken();
							base.Expect(24);
							this.GetTypeTypeName(out type);
							base.Expect(25);
							pexpr = new TypeOfExpression(type);
							goto IL_79D;
						}
						if (kind == 111)
						{
							goto IL_471;
						}
						switch (kind)
						{
						case 117:
							goto IL_471;
						case 118:
							goto IL_79D;
						case 119:
							this.lexer.NextToken();
							pexpr = new ThisReferenceExpression();
							goto IL_79D;
						default:
							goto IL_79D;
						}
					}
				}
				else if (kind <= 160)
				{
					switch (kind)
					{
					case 124:
					case 125:
					{
						Expression retExpr = null;
						if (this.la.kind == 124)
						{
							this.lexer.NextToken();
							retExpr = new BaseReferenceExpression();
						}
						else if (this.la.kind == 125)
						{
							this.lexer.NextToken();
							retExpr = new ClassReferenceExpression();
						}
						else
						{
							base.SynErr(240);
						}
						base.Expect(10);
						this.IdentifierOrKeyword(out name);
						pexpr = new FieldReferenceExpression(retExpr, name);
						goto IL_79D;
					}
					case 126:
					case 128:
					case 129:
					case 131:
					case 132:
						goto IL_79D;
					case 127:
						this.ObjectCreateExpression(out expr);
						pexpr = expr;
						goto IL_79D;
					case 130:
						this.lexer.NextToken();
						pexpr = new PrimitiveExpression(null, "null");
						goto IL_79D;
					case 133:
						goto IL_471;
					case 134:
						break;
					default:
						if (kind != 144)
						{
							switch (kind)
							{
							case 159:
							case 160:
								goto IL_471;
							default:
								goto IL_79D;
							}
						}
						break;
					}
				}
				else
				{
					if (kind == 165)
					{
						goto IL_471;
					}
					switch (kind)
					{
					case 169:
					case 176:
					case 177:
						break;
					case 170:
					case 171:
					case 172:
					case 174:
						goto IL_79D;
					case 173:
						this.lexer.NextToken();
						pexpr = new PrimitiveExpression(true, "true");
						goto IL_79D;
					case 175:
						this.lexer.NextToken();
						this.SimpleExpr(out expr);
						base.Expect(113);
						this.TypeName(out type);
						pexpr = new TypeOfIsExpression(expr, type);
						goto IL_79D;
					default:
						switch (kind)
						{
						case 190:
						case 191:
						case 192:
						case 193:
							goto IL_471;
						case 194:
						case 195:
						case 196:
						case 197:
							goto IL_6E4;
						case 198:
							this.lexer.NextToken();
							base.Expect(10);
							this.Identifier();
							type = new TypeReference(this.t.val ?? "");
							type.IsGlobal = true;
							pexpr = new TypeReferenceExpression(type);
							goto IL_79D;
						case 199:
							goto IL_616;
						case 200:
						case 201:
						case 202:
						case 203:
							goto IL_79D;
						case 204:
							break;
						default:
							goto IL_79D;
						}
						break;
					}
				}
				this.Identifier();
				pexpr = new IdentifierExpression(this.t.val);
				goto IL_79D;
				IL_471:
				string val = string.Empty;
				if (this.StartOf(10))
				{
					this.PrimitiveTypeName(out val);
				}
				else if (this.la.kind == 133)
				{
					this.lexer.NextToken();
					val = "Object";
				}
				else
				{
					base.SynErr(239);
				}
				base.Expect(10);
				this.t.val = "";
				this.Identifier();
				pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), this.t.val);
				goto IL_79D;
				IL_616:
				CastType castType = CastType.Cast;
				if (this.la.kind == 82)
				{
					this.lexer.NextToken();
				}
				else if (this.la.kind == 75)
				{
					this.lexer.NextToken();
					castType = CastType.Conversion;
				}
				else if (this.la.kind == 199)
				{
					this.lexer.NextToken();
					castType = CastType.TryCast;
				}
				else
				{
					base.SynErr(241);
				}
				base.Expect(24);
				this.Expr(out expr);
				base.Expect(12);
				this.TypeName(out type);
				base.Expect(25);
				pexpr = new CastExpression(type, expr, castType);
				goto IL_79D;
				IL_6E4:
				this.CastTarget(out type);
				base.Expect(24);
				this.Expr(out expr);
				base.Expect(25);
				pexpr = new CastExpression(type, expr, CastType.PrimitiveConversion);
				IL_79D:;
			}
			else if (this.la.kind == 10)
			{
				this.lexer.NextToken();
				this.IdentifierOrKeyword(out name);
				pexpr = new FieldReferenceExpression(null, name);
			}
			else
			{
				base.SynErr(242);
			}
		}

		private void InvocationExpression(ref Expression pexpr)
		{
			List<TypeReference> typeParameters = new List<TypeReference>();
			List<Expression> parameters = null;
			base.Expect(24);
			Location start = this.t.Location;
			if (this.la.kind == 200)
			{
				this.lexer.NextToken();
				TypeReference type;
				this.TypeName(out type);
				if (type != null)
				{
					typeParameters.Add(type);
				}
				while (this.la.kind == 12)
				{
					this.lexer.NextToken();
					this.TypeName(out type);
					if (type != null)
					{
						typeParameters.Add(type);
					}
				}
				base.Expect(25);
				if (this.la.kind == 10)
				{
					this.lexer.NextToken();
					this.Identifier();
					pexpr = new FieldReferenceExpression(this.GetTypeReferenceExpression(pexpr, typeParameters), this.t.val);
				}
				else if (this.la.kind == 24)
				{
					this.lexer.NextToken();
					this.ArgumentList(out parameters);
					base.Expect(25);
					pexpr = new InvocationExpression(pexpr, parameters, typeParameters);
				}
				else
				{
					base.SynErr(243);
				}
			}
			else if (this.StartOf(30))
			{
				this.ArgumentList(out parameters);
				base.Expect(25);
				pexpr = new InvocationExpression(pexpr, parameters, typeParameters);
			}
			else
			{
				base.SynErr(244);
			}
			pexpr.StartLocation = start;
			pexpr.EndLocation = this.t.Location;
		}

		private void PrimitiveTypeName(out string type)
		{
			type = string.Empty;
			int kind = this.la.kind;
			if (kind <= 84)
			{
				if (kind <= 65)
				{
					switch (kind)
					{
					case 52:
						this.lexer.NextToken();
						type = "Boolean";
						return;
					case 53:
						break;
					case 54:
						this.lexer.NextToken();
						type = "Byte";
						return;
					default:
						if (kind == 65)
						{
							this.lexer.NextToken();
							type = "Char";
							return;
						}
						break;
					}
				}
				else
				{
					switch (kind)
					{
					case 76:
						this.lexer.NextToken();
						type = "Date";
						return;
					case 77:
						this.lexer.NextToken();
						type = "Decimal";
						return;
					default:
						if (kind == 84)
						{
							this.lexer.NextToken();
							type = "Double";
							return;
						}
						break;
					}
				}
			}
			else if (kind <= 117)
			{
				if (kind == 111)
				{
					this.lexer.NextToken();
					type = "Integer";
					return;
				}
				if (kind == 117)
				{
					this.lexer.NextToken();
					type = "Long";
					return;
				}
			}
			else
			{
				switch (kind)
				{
				case 159:
					this.lexer.NextToken();
					type = "Short";
					return;
				case 160:
					this.lexer.NextToken();
					type = "Single";
					return;
				default:
					if (kind == 165)
					{
						this.lexer.NextToken();
						type = "String";
						return;
					}
					switch (kind)
					{
					case 190:
						this.lexer.NextToken();
						type = "SByte";
						return;
					case 191:
						this.lexer.NextToken();
						type = "UInteger";
						return;
					case 192:
						this.lexer.NextToken();
						type = "ULong";
						return;
					case 193:
						this.lexer.NextToken();
						type = "UShort";
						return;
					}
					break;
				}
			}
			base.SynErr(245);
		}

		private void CastTarget(out TypeReference type)
		{
			type = null;
			int kind = this.la.kind;
			switch (kind)
			{
			case 59:
				this.lexer.NextToken();
				type = new TypeReference("System.Boolean");
				return;
			case 60:
				this.lexer.NextToken();
				type = new TypeReference("System.Byte");
				return;
			case 61:
				this.lexer.NextToken();
				type = new TypeReference("System.Char");
				return;
			case 62:
				this.lexer.NextToken();
				type = new TypeReference("System.DateTime");
				return;
			case 63:
				this.lexer.NextToken();
				type = new TypeReference("System.Double");
				return;
			case 64:
				this.lexer.NextToken();
				type = new TypeReference("System.Decimal");
				return;
			case 65:
			case 67:
			case 70:
			case 71:
				break;
			case 66:
				this.lexer.NextToken();
				type = new TypeReference("System.Int32");
				return;
			case 68:
				this.lexer.NextToken();
				type = new TypeReference("System.Int64");
				return;
			case 69:
				this.lexer.NextToken();
				type = new TypeReference("System.Object");
				return;
			case 72:
				this.lexer.NextToken();
				type = new TypeReference("System.Int16");
				return;
			case 73:
				this.lexer.NextToken();
				type = new TypeReference("System.Single");
				return;
			case 74:
				this.lexer.NextToken();
				type = new TypeReference("System.String");
				return;
			default:
				switch (kind)
				{
				case 194:
					this.lexer.NextToken();
					type = new TypeReference("System.SByte");
					return;
				case 195:
					this.lexer.NextToken();
					type = new TypeReference("System.UInt16");
					return;
				case 196:
					this.lexer.NextToken();
					type = new TypeReference("System.UInt32");
					return;
				case 197:
					this.lexer.NextToken();
					type = new TypeReference("System.UInt64");
					return;
				}
				break;
			}
			base.SynErr(246);
		}

		private void GetTypeTypeName(out TypeReference typeref)
		{
			ArrayList rank = null;
			this.NonArrayTypeName(out typeref, true);
			this.ArrayTypeModifiers(out rank);
			if (rank != null && typeref != null)
			{
				typeref.RankSpecifier = (int[])rank.ToArray(typeof(int));
			}
		}

		private void ArgumentList(out List<Expression> arguments)
		{
			arguments = new List<Expression>();
			Expression expr = null;
			if (this.StartOf(27))
			{
				this.Argument(out expr);
			}
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				arguments.Add(expr ?? Expression.Null);
				expr = null;
				if (this.StartOf(27))
				{
					this.Argument(out expr);
				}
				if (expr == null)
				{
					expr = Expression.Null;
				}
			}
			if (expr != null)
			{
				arguments.Add(expr);
			}
		}

		private void ConjunctionExpr(out Expression outExpr)
		{
			this.NotExpr(out outExpr);
			while (this.la.kind == 45 || this.la.kind == 46)
			{
				BinaryOperatorType op;
				if (this.la.kind == 45)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.BitwiseAnd;
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.LogicalAnd;
				}
				Expression expr;
				this.NotExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void NotExpr(out Expression outExpr)
		{
			UnaryOperatorType uop = UnaryOperatorType.None;
			while (this.la.kind == 129)
			{
				this.lexer.NextToken();
				uop = UnaryOperatorType.Not;
			}
			this.ComparisonExpr(out outExpr);
			if (uop != UnaryOperatorType.None)
			{
				outExpr = new UnaryOperatorExpression(outExpr, uop);
			}
		}

		private void ComparisonExpr(out Expression outExpr)
		{
			BinaryOperatorType op = BinaryOperatorType.None;
			this.ShiftExpr(out outExpr);
			while (this.StartOf(31))
			{
				int kind = this.la.kind;
				if (kind <= 30)
				{
					if (kind != 11)
					{
						switch (kind)
						{
						case 26:
							this.lexer.NextToken();
							op = BinaryOperatorType.GreaterThan;
							break;
						case 27:
							this.lexer.NextToken();
							op = BinaryOperatorType.LessThan;
							break;
						case 28:
							this.lexer.NextToken();
							op = BinaryOperatorType.InEquality;
							break;
						case 29:
							this.lexer.NextToken();
							op = BinaryOperatorType.GreaterThanOrEqual;
							break;
						case 30:
							this.lexer.NextToken();
							op = BinaryOperatorType.LessThanOrEqual;
							break;
						}
					}
					else
					{
						this.lexer.NextToken();
						op = BinaryOperatorType.Equality;
					}
				}
				else if (kind != 113)
				{
					if (kind != 116)
					{
						if (kind == 189)
						{
							this.lexer.NextToken();
							op = BinaryOperatorType.ReferenceInequality;
						}
					}
					else
					{
						this.lexer.NextToken();
						op = BinaryOperatorType.Like;
					}
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.ReferenceEquality;
				}
				if (this.StartOf(32))
				{
					Expression expr;
					this.ShiftExpr(out expr);
					outExpr = new BinaryOperatorExpression(outExpr, op, expr);
				}
				else if (this.la.kind == 129)
				{
					this.lexer.NextToken();
					Expression expr;
					this.ShiftExpr(out expr);
					outExpr = new BinaryOperatorExpression(outExpr, op, new UnaryOperatorExpression(expr, UnaryOperatorType.Not));
				}
				else
				{
					base.SynErr(247);
				}
			}
		}

		private void ShiftExpr(out Expression outExpr)
		{
			this.ConcatenationExpr(out outExpr);
			while (this.la.kind == 31 || this.la.kind == 32)
			{
				BinaryOperatorType op;
				if (this.la.kind == 31)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.ShiftLeft;
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.ShiftRight;
				}
				Expression expr;
				this.ConcatenationExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void ConcatenationExpr(out Expression outExpr)
		{
			this.AdditiveExpr(out outExpr);
			while (this.la.kind == 19)
			{
				this.lexer.NextToken();
				Expression expr;
				this.AdditiveExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Concat, expr);
			}
		}

		private void AdditiveExpr(out Expression outExpr)
		{
			this.ModuloExpr(out outExpr);
			while (this.la.kind == 14 || this.la.kind == 15)
			{
				BinaryOperatorType op;
				if (this.la.kind == 14)
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
				this.ModuloExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void ModuloExpr(out Expression outExpr)
		{
			this.IntegerDivisionExpr(out outExpr);
			while (this.la.kind == 120)
			{
				this.lexer.NextToken();
				Expression expr;
				this.IntegerDivisionExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Modulus, expr);
			}
		}

		private void IntegerDivisionExpr(out Expression outExpr)
		{
			this.MultiplicativeExpr(out outExpr);
			while (this.la.kind == 18)
			{
				this.lexer.NextToken();
				Expression expr;
				this.MultiplicativeExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.DivideInteger, expr);
			}
		}

		private void MultiplicativeExpr(out Expression outExpr)
		{
			this.UnaryExpr(out outExpr);
			while (this.la.kind == 16 || this.la.kind == 17)
			{
				BinaryOperatorType op;
				if (this.la.kind == 16)
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Multiply;
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Divide;
				}
				Expression expr;
				this.UnaryExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);
			}
		}

		private void UnaryExpr(out Expression uExpr)
		{
			UnaryOperatorType uop = UnaryOperatorType.None;
			bool isUOp = false;
			while (this.la.kind == 14 || this.la.kind == 15 || this.la.kind == 16)
			{
				if (this.la.kind == 14)
				{
					this.lexer.NextToken();
					uop = UnaryOperatorType.Plus;
					isUOp = true;
				}
				else if (this.la.kind == 15)
				{
					this.lexer.NextToken();
					uop = UnaryOperatorType.Minus;
					isUOp = true;
				}
				else
				{
					this.lexer.NextToken();
					uop = UnaryOperatorType.Star;
					isUOp = true;
				}
			}
			Expression expr;
			this.ExponentiationExpr(out expr);
			if (isUOp)
			{
				uExpr = new UnaryOperatorExpression(expr, uop);
			}
			else
			{
				uExpr = expr;
			}
		}

		private void ExponentiationExpr(out Expression outExpr)
		{
			this.SimpleExpr(out outExpr);
			while (this.la.kind == 20)
			{
				this.lexer.NextToken();
				Expression expr;
				this.SimpleExpr(out expr);
				outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.Power, expr);
			}
		}

		private void NormalOrReDimArgumentList(out List<Expression> arguments, out bool canBeNormal, out bool canBeRedim)
		{
			arguments = new List<Expression>();
			canBeNormal = true;
			canBeRedim = !this.IsNamedAssign();
			Expression expr = null;
			if (this.StartOf(27))
			{
				this.Argument(out expr);
				if (this.la.kind == 172)
				{
					this.lexer.NextToken();
					this.EnsureIsZero(expr);
					canBeNormal = false;
					this.Expr(out expr);
				}
			}
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				if (expr == null)
				{
					canBeRedim = false;
				}
				arguments.Add(expr ?? Expression.Null);
				expr = null;
				canBeRedim &= !this.IsNamedAssign();
				if (this.StartOf(27))
				{
					this.Argument(out expr);
					if (this.la.kind == 172)
					{
						this.lexer.NextToken();
						this.EnsureIsZero(expr);
						canBeNormal = false;
						this.Expr(out expr);
					}
				}
				if (expr == null)
				{
					canBeRedim = false;
					expr = Expression.Null;
				}
			}
			if (expr != null)
			{
				arguments.Add(expr);
			}
			else
			{
				canBeRedim = false;
			}
		}

		private void ArrayTypeModifiers(out ArrayList arrayModifiers)
		{
			arrayModifiers = new ArrayList();
			int i = 0;
			while (this.IsDims())
			{
				base.Expect(24);
				if (this.la.kind == 12 || this.la.kind == 25)
				{
					this.RankList(out i);
				}
				arrayModifiers.Add(i);
				base.Expect(25);
			}
			if (arrayModifiers.Count == 0)
			{
				arrayModifiers = null;
			}
		}

		private void Argument(out Expression argumentexpr)
		{
			argumentexpr = null;
			if (this.IsNamedAssign())
			{
				this.Identifier();
				string name = this.t.val;
				base.Expect(13);
				base.Expect(11);
				Expression expr;
				this.Expr(out expr);
				argumentexpr = new NamedArgumentExpression(name, expr);
			}
			else if (this.StartOf(27))
			{
				this.Expr(out argumentexpr);
			}
			else
			{
				base.SynErr(248);
			}
		}

		private void QualIdentAndTypeArguments(out TypeReference typeref, bool canBeUnbound)
		{
			typeref = null;
			string name;
			this.Qualident(out name);
			typeref = new TypeReference(name);
			if (this.la.kind == 24 && this.Peek(1).kind == 200)
			{
				this.lexer.NextToken();
				base.Expect(200);
				if (canBeUnbound && (this.la.kind == 25 || this.la.kind == 12))
				{
					typeref.GenericTypes.Add(NullTypeReference.Instance);
					while (this.la.kind == 12)
					{
						this.lexer.NextToken();
						typeref.GenericTypes.Add(NullTypeReference.Instance);
					}
				}
				else if (this.StartOf(6))
				{
					this.TypeArgumentList(typeref.GenericTypes);
				}
				else
				{
					base.SynErr(249);
				}
				base.Expect(25);
			}
		}

		private void TypeArgumentList(List<TypeReference> typeArguments)
		{
			TypeReference typeref;
			this.TypeName(out typeref);
			if (typeref != null)
			{
				typeArguments.Add(typeref);
			}
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.TypeName(out typeref);
				if (typeref != null)
				{
					typeArguments.Add(typeref);
				}
			}
		}

		private void RankList(out int i)
		{
			i = 0;
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				i++;
			}
		}

		private void Attribute(out AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute)
		{
			List<Expression> positional = new List<Expression>();
			List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
			if (this.la.kind == 198)
			{
				this.lexer.NextToken();
				base.Expect(10);
			}
			string name;
			this.Qualident(out name);
			if (this.la.kind == 24)
			{
				this.AttributeArguments(positional, named);
			}
			attribute = new AIMS.Libraries.Scripting.NRefactory.Ast.Attribute(name, positional, named);
		}

		private void AttributeArguments(List<Expression> positional, List<NamedArgumentExpression> named)
		{
			bool nameFound = false;
			string name = "";
			base.Expect(24);
			if (this.IsNotClosingParenthesis())
			{
				if (this.IsNamedAssign())
				{
					nameFound = true;
					this.IdentifierOrKeyword(out name);
					if (this.la.kind == 13)
					{
						this.lexer.NextToken();
					}
					base.Expect(11);
				}
				Expression expr;
				this.Expr(out expr);
				if (expr != null)
				{
					if (string.IsNullOrEmpty(name))
					{
						positional.Add(expr);
					}
					else
					{
						named.Add(new NamedArgumentExpression(name, expr));
						name = "";
					}
				}
				while (this.la.kind == 12)
				{
					this.lexer.NextToken();
					if (this.IsNamedAssign())
					{
						nameFound = true;
						this.IdentifierOrKeyword(out name);
						if (this.la.kind == 13)
						{
							this.lexer.NextToken();
						}
						base.Expect(11);
					}
					else if (this.StartOf(27))
					{
						if (nameFound)
						{
							this.Error("no positional argument after named argument");
						}
					}
					else
					{
						base.SynErr(250);
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
			base.Expect(25);
		}

		private void FormalParameter(out ParameterDeclarationExpression p)
		{
			TypeReference type = null;
			ParamModifierList mod = new ParamModifierList(this);
			Expression expr = null;
			p = null;
			ArrayList arrayModifiers = null;
			while (this.StartOf(33))
			{
				this.ParameterModifier(mod);
			}
			this.Identifier();
			string parameterName = this.t.val;
			if (this.IsDims())
			{
				this.ArrayTypeModifiers(out arrayModifiers);
			}
			if (this.la.kind == 48)
			{
				this.lexer.NextToken();
				this.TypeName(out type);
			}
			if (type != null)
			{
				if (arrayModifiers != null)
				{
					if (type.RankSpecifier != null)
					{
						this.Error("array rank only allowed one time");
					}
					else
					{
						type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
					}
				}
			}
			else
			{
				type = new TypeReference("System.Object", (arrayModifiers == null) ? null : ((int[])arrayModifiers.ToArray(typeof(int))));
			}
			if (this.la.kind == 11)
			{
				this.lexer.NextToken();
				this.Expr(out expr);
			}
			mod.Check();
			p = new ParameterDeclarationExpression(type, parameterName, mod.Modifier, expr);
		}

		private void ParameterModifier(ParamModifierList m)
		{
			if (this.la.kind == 55)
			{
				this.lexer.NextToken();
				m.Add(ParameterModifiers.In);
			}
			else if (this.la.kind == 53)
			{
				this.lexer.NextToken();
				m.Add(ParameterModifiers.Ref);
			}
			else if (this.la.kind == 137)
			{
				this.lexer.NextToken();
				m.Add(ParameterModifiers.Optional);
			}
			else if (this.la.kind == 143)
			{
				this.lexer.NextToken();
				m.Add(ParameterModifiers.Params);
			}
			else
			{
				base.SynErr(251);
			}
		}

		private void Statement()
		{
			Statement stmt = null;
			Location startPos = this.la.Location;
			string label = string.Empty;
			if (this.la.kind != 1 && this.la.kind != 13)
			{
				if (this.IsLabel())
				{
					this.LabelName(out label);
					this.compilationUnit.AddChild(new LabelStatement(this.t.val));
					base.Expect(13);
					this.Statement();
				}
				else if (this.StartOf(34))
				{
					this.EmbeddedStatement(out stmt);
					this.compilationUnit.AddChild(stmt);
				}
				else if (this.StartOf(35))
				{
					this.LocalDeclarationStatement(out stmt);
					this.compilationUnit.AddChild(stmt);
				}
				else
				{
					base.SynErr(252);
				}
			}
			if (stmt != null)
			{
				stmt.StartLocation = startPos;
				stmt.EndLocation = this.t.Location;
			}
		}

		private void LabelName(out string name)
		{
			name = string.Empty;
			if (this.StartOf(13))
			{
				this.Identifier();
				name = this.t.val;
			}
			else if (this.la.kind == 5)
			{
				this.lexer.NextToken();
				name = this.t.val;
			}
			else
			{
				base.SynErr(253);
			}
		}

		private void EmbeddedStatement(out Statement statement)
		{
			Statement embeddedStatement = null;
			statement = null;
			Expression expr = null;
			string name = string.Empty;
			List<Expression> p = null;
			int kind = this.la.kind;
			switch (kind)
			{
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
				break;
			default:
				if (kind != 24)
				{
					switch (kind)
					{
					case 42:
					{
						this.lexer.NextToken();
						Expression handlerExpr = null;
						this.Expr(out expr);
						base.Expect(12);
						this.Expr(out handlerExpr);
						statement = new AddHandlerStatement(expr, handlerExpr);
						return;
					}
					case 43:
					case 47:
					case 49:
					case 50:
					case 51:
					case 52:
					case 54:
					case 59:
					case 60:
					case 61:
					case 62:
					case 63:
					case 64:
					case 65:
					case 66:
					case 68:
					case 69:
					case 70:
					case 72:
					case 73:
					case 74:
					case 75:
					case 76:
					case 77:
					case 82:
					case 84:
					case 95:
					case 96:
					case 102:
					case 111:
					case 117:
					case 119:
					case 124:
					case 125:
					case 127:
					case 130:
					case 133:
					case 134:
					case 144:
					case 159:
					case 160:
					case 165:
					case 169:
					case 173:
					case 175:
					case 176:
					case 177:
					case 190:
					case 191:
					case 192:
					case 193:
					case 194:
					case 195:
					case 196:
					case 197:
					case 198:
					case 199:
					case 204:
						goto IL_FC7;
					case 56:
						this.lexer.NextToken();
						this.SimpleExpr(out expr);
						statement = new ExpressionStatement(expr);
						return;
					case 83:
					{
						this.lexer.NextToken();
						ConditionType conditionType = ConditionType.None;
						if (this.la.kind == 177 || this.la.kind == 181)
						{
							this.WhileOrUntil(out conditionType);
							this.Expr(out expr);
							this.EndOfStmt();
							this.Block(out embeddedStatement);
							base.Expect(118);
							statement = new DoLoopStatement(expr, embeddedStatement, (conditionType == ConditionType.While) ? ConditionType.DoWhile : conditionType, ConditionPosition.Start);
						}
						else if (this.la.kind == 1 || this.la.kind == 13)
						{
							this.EndOfStmt();
							this.Block(out embeddedStatement);
							base.Expect(118);
							if (this.la.kind == 177 || this.la.kind == 181)
							{
								this.WhileOrUntil(out conditionType);
								this.Expr(out expr);
							}
							statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
						}
						else
						{
							base.SynErr(255);
						}
						return;
					}
					case 91:
					{
						this.lexer.NextToken();
						this.Expr(out expr);
						List<Expression> arrays = new List<Expression>();
						if (expr != null)
						{
							arrays.Add(expr);
						}
						EraseStatement eraseStatement = new EraseStatement(arrays);
						while (this.la.kind == 12)
						{
							this.lexer.NextToken();
							this.Expr(out expr);
							if (expr != null)
							{
								arrays.Add(expr);
							}
						}
						statement = eraseStatement;
						return;
					}
					case 92:
						this.lexer.NextToken();
						this.Expr(out expr);
						statement = new ErrorStatement(expr);
						return;
					case 94:
					{
						this.lexer.NextToken();
						ExitType exitType = ExitType.None;
						kind = this.la.kind;
						if (kind <= 146)
						{
							if (kind == 83)
							{
								this.lexer.NextToken();
								exitType = ExitType.Do;
								goto IL_41C;
							}
							switch (kind)
							{
							case 98:
								this.lexer.NextToken();
								exitType = ExitType.For;
								goto IL_41C;
							case 99:
								break;
							case 100:
								this.lexer.NextToken();
								exitType = ExitType.Function;
								goto IL_41C;
							default:
								if (kind == 146)
								{
									this.lexer.NextToken();
									exitType = ExitType.Property;
									goto IL_41C;
								}
								break;
							}
						}
						else if (kind <= 167)
						{
							if (kind == 155)
							{
								this.lexer.NextToken();
								exitType = ExitType.Select;
								goto IL_41C;
							}
							if (kind == 167)
							{
								this.lexer.NextToken();
								exitType = ExitType.Sub;
								goto IL_41C;
							}
						}
						else
						{
							if (kind == 174)
							{
								this.lexer.NextToken();
								exitType = ExitType.Try;
								goto IL_41C;
							}
							if (kind == 181)
							{
								this.lexer.NextToken();
								exitType = ExitType.While;
								goto IL_41C;
							}
						}
						base.SynErr(254);
						IL_41C:
						statement = new ExitStatement(exitType);
						return;
					}
					case 98:
					{
						this.lexer.NextToken();
						Expression group = null;
						Location startLocation = this.t.Location;
						if (this.la.kind == 85)
						{
							this.lexer.NextToken();
							TypeReference typeReference;
							string typeName;
							this.LoopControlVariable(out typeReference, out typeName);
							base.Expect(109);
							this.Expr(out group);
							this.EndOfStmt();
							this.Block(out embeddedStatement);
							base.Expect(128);
							if (this.StartOf(27))
							{
								this.Expr(out expr);
							}
							statement = new ForeachStatement(typeReference, typeName, group, embeddedStatement, expr);
							statement.StartLocation = startLocation;
							statement.EndLocation = this.t.EndLocation;
						}
						else if (this.StartOf(13))
						{
							Expression start = null;
							Expression end = null;
							Expression step = null;
							Expression nextExpr = null;
							List<Expression> nextExpressions = null;
							TypeReference typeReference;
							string typeName;
							this.LoopControlVariable(out typeReference, out typeName);
							base.Expect(11);
							this.Expr(out start);
							base.Expect(172);
							this.Expr(out end);
							if (this.la.kind == 162)
							{
								this.lexer.NextToken();
								this.Expr(out step);
							}
							this.EndOfStmt();
							this.Block(out embeddedStatement);
							base.Expect(128);
							if (this.StartOf(27))
							{
								this.Expr(out nextExpr);
								nextExpressions = new List<Expression>();
								nextExpressions.Add(nextExpr);
								while (this.la.kind == 12)
								{
									this.lexer.NextToken();
									this.Expr(out nextExpr);
									nextExpressions.Add(nextExpr);
								}
							}
							statement = new ForNextStatement(typeReference, typeName, start, end, step, embeddedStatement, nextExpressions);
						}
						else
						{
							base.SynErr(256);
						}
						return;
					}
					case 104:
					{
						GotoStatement goToStatement = null;
						this.GotoStatement(out goToStatement);
						statement = goToStatement;
						return;
					}
					case 106:
					{
						this.lexer.NextToken();
						Location ifStartLocation = this.t.Location;
						this.Expr(out expr);
						if (this.la.kind == 170)
						{
							this.lexer.NextToken();
						}
						if (this.la.kind == 1 || this.la.kind == 13)
						{
							this.EndOfStmt();
							this.Block(out embeddedStatement);
							IfElseStatement ifStatement = new IfElseStatement(expr, embeddedStatement);
							ifStatement.StartLocation = ifStartLocation;
							while (this.la.kind == 87 || this.IsElseIf())
							{
								Location elseIfStart;
								if (this.IsElseIf())
								{
									base.Expect(86);
									elseIfStart = this.t.Location;
									base.Expect(106);
								}
								else
								{
									this.lexer.NextToken();
									elseIfStart = this.t.Location;
								}
								Expression condition = null;
								Statement block = null;
								this.Expr(out condition);
								if (this.la.kind == 170)
								{
									this.lexer.NextToken();
								}
								this.EndOfStmt();
								this.Block(out block);
								ElseIfSection elseIfSection = new ElseIfSection(condition, block);
								elseIfSection.StartLocation = elseIfStart;
								elseIfSection.EndLocation = this.t.Location;
								elseIfSection.Parent = ifStatement;
								ifStatement.ElseIfSections.Add(elseIfSection);
							}
							if (this.la.kind == 86)
							{
								this.lexer.NextToken();
								this.EndOfStmt();
								this.Block(out embeddedStatement);
								ifStatement.FalseStatement.Add(embeddedStatement);
							}
							base.Expect(88);
							base.Expect(106);
							ifStatement.EndLocation = this.t.Location;
							statement = ifStatement;
						}
						else if (this.StartOf(36))
						{
							IfElseStatement ifStatement = new IfElseStatement(expr);
							ifStatement.StartLocation = ifStartLocation;
							this.SingleLineStatementList(ifStatement.TrueStatement);
							if (this.la.kind == 86)
							{
								this.lexer.NextToken();
								if (this.StartOf(36))
								{
									this.SingleLineStatementList(ifStatement.FalseStatement);
								}
							}
							ifStatement.EndLocation = this.t.Location;
							statement = ifStatement;
						}
						else
						{
							base.SynErr(257);
						}
						return;
					}
					case 135:
					{
						OnErrorStatement onErrorStatement = null;
						this.OnErrorStatement(out onErrorStatement);
						statement = onErrorStatement;
						return;
					}
					case 149:
						this.lexer.NextToken();
						this.Identifier();
						name = this.t.val;
						if (this.la.kind == 24)
						{
							this.lexer.NextToken();
							if (this.StartOf(30))
							{
								this.ArgumentList(out p);
							}
							base.Expect(25);
						}
						statement = new RaiseEventStatement(name, p);
						return;
					case 151:
					{
						this.lexer.NextToken();
						bool isPreserve = false;
						if (this.la.kind == 144)
						{
							this.lexer.NextToken();
							isPreserve = true;
						}
						this.ReDimClause(out expr);
						ReDimStatement reDimStatement = new ReDimStatement(isPreserve);
						statement = reDimStatement;
						InvocationExpression redimClause = expr as InvocationExpression;
						if (redimClause != null)
						{
							reDimStatement.ReDimClauses.Add(redimClause);
						}
						while (this.la.kind == 12)
						{
							this.lexer.NextToken();
							this.ReDimClause(out expr);
							redimClause = (expr as InvocationExpression);
							if (redimClause != null)
							{
								reDimStatement.ReDimClauses.Add(redimClause);
							}
						}
						return;
					}
					case 152:
					{
						this.lexer.NextToken();
						Expression handlerExpr = null;
						this.Expr(out expr);
						base.Expect(12);
						this.Expr(out handlerExpr);
						statement = new RemoveHandlerStatement(expr, handlerExpr);
						return;
					}
					case 153:
					{
						ResumeStatement resumeStatement = null;
						this.ResumeStatement(out resumeStatement);
						statement = resumeStatement;
						return;
					}
					case 154:
						this.lexer.NextToken();
						if (this.StartOf(27))
						{
							this.Expr(out expr);
						}
						statement = new ReturnStatement(expr);
						return;
					case 155:
					{
						this.lexer.NextToken();
						if (this.la.kind == 57)
						{
							this.lexer.NextToken();
						}
						this.Expr(out expr);
						this.EndOfStmt();
						List<SwitchSection> selectSections = new List<SwitchSection>();
						Statement block = null;
						while (this.la.kind == 57)
						{
							List<CaseLabel> caseClauses = null;
							Location caseLocation = this.la.Location;
							this.lexer.NextToken();
							this.CaseClauses(out caseClauses);
							if (this.IsNotStatementSeparator())
							{
								this.lexer.NextToken();
							}
							this.EndOfStmt();
							SwitchSection selectSection = new SwitchSection(caseClauses);
							selectSection.StartLocation = caseLocation;
							this.Block(out block);
							selectSection.Children = block.Children;
							selectSection.EndLocation = this.t.EndLocation;
							selectSections.Add(selectSection);
						}
						statement = new SwitchStatement(expr, selectSections);
						base.Expect(88);
						base.Expect(155);
						return;
					}
					case 163:
						this.lexer.NextToken();
						statement = new StopStatement();
						return;
					case 168:
						this.lexer.NextToken();
						this.Expr(out expr);
						this.EndOfStmt();
						this.Block(out embeddedStatement);
						base.Expect(88);
						base.Expect(168);
						statement = new LockStatement(expr, embeddedStatement);
						return;
					case 171:
						this.lexer.NextToken();
						if (this.StartOf(27))
						{
							this.Expr(out expr);
						}
						statement = new ThrowStatement(expr);
						return;
					case 174:
						this.TryStatement(out statement);
						return;
					case 181:
						this.lexer.NextToken();
						this.Expr(out expr);
						this.EndOfStmt();
						this.Block(out embeddedStatement);
						base.Expect(88);
						base.Expect(181);
						statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
						return;
					case 182:
						this.WithStatement(out statement);
						return;
					case 186:
					{
						this.lexer.NextToken();
						ContinueType continueType = ContinueType.None;
						if (this.la.kind == 83 || this.la.kind == 98 || this.la.kind == 181)
						{
							if (this.la.kind == 83)
							{
								this.lexer.NextToken();
								continueType = ContinueType.Do;
							}
							else if (this.la.kind == 98)
							{
								this.lexer.NextToken();
								continueType = ContinueType.For;
							}
							else
							{
								this.lexer.NextToken();
								continueType = ContinueType.While;
							}
						}
						statement = new ContinueStatement(continueType);
						return;
					}
					case 188:
						this.lexer.NextToken();
						if (this.Peek(1).kind == 48)
						{
							LocalVariableDeclaration resourceAquisition = new LocalVariableDeclaration(Modifiers.None);
							this.VariableDeclarator(resourceAquisition.Variables);
							while (this.la.kind == 12)
							{
								this.lexer.NextToken();
								this.VariableDeclarator(resourceAquisition.Variables);
							}
							Statement block;
							this.Block(out block);
							statement = new UsingStatement(resourceAquisition, block);
						}
						else if (this.StartOf(27))
						{
							this.Expr(out expr);
							Statement block;
							this.Block(out block);
							statement = new UsingStatement(new ExpressionStatement(expr), block);
						}
						else
						{
							base.SynErr(259);
						}
						base.Expect(88);
						base.Expect(188);
						return;
					}
					base.SynErr(260);
					return;
				}
				break;
			}
			IL_FC7:
			Expression val = null;
			bool mustBeAssignment = this.la.kind == 14 || this.la.kind == 15 || this.la.kind == 129 || this.la.kind == 16;
			this.SimpleExpr(out expr);
			if (this.StartOf(37))
			{
				AssignmentOperatorType op;
				this.AssignmentOperator(out op);
				this.Expr(out val);
				expr = new AssignmentExpression(expr, op, val);
			}
			else if (this.la.kind == 1 || this.la.kind == 13 || this.la.kind == 86)
			{
				if (mustBeAssignment)
				{
					this.Error("error in assignment.");
				}
			}
			else
			{
				base.SynErr(258);
			}
			if (expr is FieldReferenceExpression || expr is IdentifierExpression)
			{
				expr = new InvocationExpression(expr);
			}
			statement = new ExpressionStatement(expr);
		}

		private void LocalDeclarationStatement(out Statement statement)
		{
			ModifierList i = new ModifierList();
			bool dimfound = false;
			while (this.la.kind == 71 || this.la.kind == 81 || this.la.kind == 161)
			{
				if (this.la.kind == 71)
				{
					this.lexer.NextToken();
					i.Add(Modifiers.Const, this.t.Location);
				}
				else if (this.la.kind == 161)
				{
					this.lexer.NextToken();
					i.Add(Modifiers.Static, this.t.Location);
				}
				else
				{
					this.lexer.NextToken();
					dimfound = true;
				}
			}
			if (dimfound && (i.Modifier & Modifiers.Const) != Modifiers.None)
			{
				this.Error("Dim is not allowed on constants.");
			}
			if (i.isNone && !dimfound)
			{
				this.Error("Const, Dim or Static expected");
			}
			LocalVariableDeclaration localVariableDeclaration = new LocalVariableDeclaration(i.Modifier);
			localVariableDeclaration.StartLocation = this.t.Location;
			this.VariableDeclarator(localVariableDeclaration.Variables);
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.VariableDeclarator(localVariableDeclaration.Variables);
			}
			statement = localVariableDeclaration;
		}

		private void TryStatement(out Statement tryStatement)
		{
			Statement blockStmt = null;
			Statement finallyStmt = null;
			List<CatchClause> catchClauses = null;
			base.Expect(174);
			this.EndOfStmt();
			this.Block(out blockStmt);
			if (this.la.kind == 58 || this.la.kind == 88 || this.la.kind == 97)
			{
				this.CatchClauses(out catchClauses);
			}
			if (this.la.kind == 97)
			{
				this.lexer.NextToken();
				this.EndOfStmt();
				this.Block(out finallyStmt);
			}
			base.Expect(88);
			base.Expect(174);
			tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		}

		private void WithStatement(out Statement withStatement)
		{
			Statement blockStmt = null;
			Expression expr = null;
			base.Expect(182);
			Location start = this.t.Location;
			this.Expr(out expr);
			this.EndOfStmt();
			withStatement = new WithStatement(expr);
			withStatement.StartLocation = start;
			this.Block(out blockStmt);
			((WithStatement)withStatement).Body = (BlockStatement)blockStmt;
			base.Expect(88);
			base.Expect(182);
			withStatement.EndLocation = this.t.Location;
		}

		private void WhileOrUntil(out ConditionType conditionType)
		{
			conditionType = ConditionType.None;
			if (this.la.kind == 181)
			{
				this.lexer.NextToken();
				conditionType = ConditionType.While;
			}
			else if (this.la.kind == 177)
			{
				this.lexer.NextToken();
				conditionType = ConditionType.Until;
			}
			else
			{
				base.SynErr(261);
			}
		}

		private void LoopControlVariable(out TypeReference type, out string name)
		{
			ArrayList arrayModifiers = null;
			type = null;
			this.Qualident(out name);
			if (this.IsDims())
			{
				this.ArrayTypeModifiers(out arrayModifiers);
			}
			if (this.la.kind == 48)
			{
				this.lexer.NextToken();
				this.TypeName(out type);
				if (name.IndexOf('.') > 0)
				{
					this.Error("No type def for 'for each' member indexer allowed.");
				}
			}
			if (type != null)
			{
				if (type.RankSpecifier != null && arrayModifiers != null)
				{
					this.Error("array rank only allowed one time");
				}
				else if (arrayModifiers != null)
				{
					type.RankSpecifier = (int[])arrayModifiers.ToArray(typeof(int));
				}
			}
		}

		private void ReDimClause(out Expression expr)
		{
			this.SimpleNonInvocationExpression(out expr);
			this.ReDimClauseInternal(ref expr);
		}

		private void SingleLineStatementList(List<Statement> list)
		{
			Statement embeddedStatement = null;
			if (this.la.kind == 88)
			{
				this.lexer.NextToken();
				embeddedStatement = new EndStatement();
			}
			else if (this.StartOf(34))
			{
				this.EmbeddedStatement(out embeddedStatement);
			}
			else
			{
				base.SynErr(262);
			}
			if (embeddedStatement != null)
			{
				list.Add(embeddedStatement);
			}
			while (this.la.kind == 13)
			{
				this.lexer.NextToken();
				while (this.la.kind == 13)
				{
					this.lexer.NextToken();
				}
				if (this.la.kind == 88)
				{
					this.lexer.NextToken();
					embeddedStatement = new EndStatement();
				}
				else if (this.StartOf(34))
				{
					this.EmbeddedStatement(out embeddedStatement);
				}
				else
				{
					base.SynErr(263);
				}
				if (embeddedStatement != null)
				{
					list.Add(embeddedStatement);
				}
			}
		}

		private void CaseClauses(out List<CaseLabel> caseClauses)
		{
			caseClauses = new List<CaseLabel>();
			CaseLabel caseClause = null;
			this.CaseClause(out caseClause);
			if (caseClause != null)
			{
				caseClauses.Add(caseClause);
			}
			while (this.la.kind == 12)
			{
				this.lexer.NextToken();
				this.CaseClause(out caseClause);
				if (caseClause != null)
				{
					caseClauses.Add(caseClause);
				}
			}
		}

		private void OnErrorStatement(out OnErrorStatement stmt)
		{
			stmt = null;
			GotoStatement goToStatement = null;
			base.Expect(135);
			base.Expect(92);
			if (this.IsNegativeLabelName())
			{
				base.Expect(104);
				base.Expect(15);
				base.Expect(5);
				long intLabel = long.Parse(this.t.val);
				if (intLabel != 1L)
				{
					this.Error("invalid label in on error statement.");
				}
				stmt = new OnErrorStatement(new GotoStatement((intLabel * -1L).ToString()));
			}
			else if (this.la.kind == 104)
			{
				this.GotoStatement(out goToStatement);
				string val = goToStatement.Label;
				try
				{
					long intLabel = long.Parse(val);
					if (intLabel != 0L)
					{
						this.Error("invalid label in on error statement.");
					}
				}
				catch
				{
				}
				stmt = new OnErrorStatement(goToStatement);
			}
			else if (this.la.kind == 153)
			{
				this.lexer.NextToken();
				base.Expect(128);
				stmt = new OnErrorStatement(new ResumeStatement(true));
			}
			else
			{
				base.SynErr(264);
			}
		}

		private void GotoStatement(out GotoStatement goToStatement)
		{
			string label = string.Empty;
			base.Expect(104);
			this.LabelName(out label);
			goToStatement = new GotoStatement(label);
		}

		private void ResumeStatement(out ResumeStatement resumeStatement)
		{
			resumeStatement = null;
			string label = string.Empty;
			if (this.IsResumeNext())
			{
				base.Expect(153);
				base.Expect(128);
				resumeStatement = new ResumeStatement(true);
			}
			else if (this.la.kind == 153)
			{
				this.lexer.NextToken();
				if (this.StartOf(38))
				{
					this.LabelName(out label);
				}
				resumeStatement = new ResumeStatement(label);
			}
			else
			{
				base.SynErr(265);
			}
		}

		private void ReDimClauseInternal(ref Expression expr)
		{
			while (this.la.kind == 10 || (this.la.kind == 24 && this.Peek(1).kind == 200))
			{
				if (this.la.kind == 10)
				{
					this.lexer.NextToken();
					string name;
					this.IdentifierOrKeyword(out name);
					expr = new FieldReferenceExpression(expr, name);
				}
				else
				{
					this.InvocationExpression(ref expr);
				}
			}
			base.Expect(24);
			List<Expression> arguments;
			bool canBeNormal;
			bool canBeRedim;
			this.NormalOrReDimArgumentList(out arguments, out canBeNormal, out canBeRedim);
			base.Expect(25);
			expr = new InvocationExpression(expr, arguments);
			if (!canBeRedim || (canBeNormal && (this.la.kind == 10 || this.la.kind == 24)))
			{
				if (base.Errors.Count == 0)
				{
					this.ReDimClauseInternal(ref expr);
				}
			}
		}

		private void CaseClause(out CaseLabel caseClause)
		{
			Expression expr = null;
			Expression sexpr = null;
			BinaryOperatorType op = BinaryOperatorType.None;
			caseClause = null;
			if (this.la.kind == 86)
			{
				this.lexer.NextToken();
				caseClause = new CaseLabel();
			}
			else if (this.StartOf(39))
			{
				if (this.la.kind == 113)
				{
					this.lexer.NextToken();
				}
				int kind = this.la.kind;
				if (kind != 11)
				{
					switch (kind)
					{
					case 26:
						this.lexer.NextToken();
						op = BinaryOperatorType.GreaterThan;
						break;
					case 27:
						this.lexer.NextToken();
						op = BinaryOperatorType.LessThan;
						break;
					case 28:
						this.lexer.NextToken();
						op = BinaryOperatorType.InEquality;
						break;
					case 29:
						this.lexer.NextToken();
						op = BinaryOperatorType.GreaterThanOrEqual;
						break;
					case 30:
						this.lexer.NextToken();
						op = BinaryOperatorType.LessThanOrEqual;
						break;
					default:
						base.SynErr(266);
						break;
					}
				}
				else
				{
					this.lexer.NextToken();
					op = BinaryOperatorType.Equality;
				}
				this.Expr(out expr);
				caseClause = new CaseLabel(op, expr);
			}
			else if (this.StartOf(27))
			{
				this.Expr(out expr);
				if (this.la.kind == 172)
				{
					this.lexer.NextToken();
					this.Expr(out sexpr);
				}
				caseClause = new CaseLabel(expr, sexpr);
			}
			else
			{
				base.SynErr(267);
			}
		}

		private void CatchClauses(out List<CatchClause> catchClauses)
		{
			catchClauses = new List<CatchClause>();
			TypeReference type = null;
			Statement blockStmt = null;
			Expression expr = null;
			string name = string.Empty;
			while (this.la.kind == 58)
			{
				this.lexer.NextToken();
				if (this.StartOf(13))
				{
					this.Identifier();
					name = this.t.val;
					if (this.la.kind == 48)
					{
						this.lexer.NextToken();
						this.TypeName(out type);
					}
				}
				if (this.la.kind == 180)
				{
					this.lexer.NextToken();
					this.Expr(out expr);
				}
				this.EndOfStmt();
				this.Block(out blockStmt);
				catchClauses.Add(new CatchClause(type, name, blockStmt, expr));
			}
		}

		public override void Parse()
		{
			this.VBNET();
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
				s = "EOL expected";
				break;
			case 2:
				s = "ident expected";
				break;
			case 3:
				s = "Literal String expected";
				break;
			case 4:
				s = "Literal Character expected";
				break;
			case 5:
				s = "Literal Integer expected";
				break;
			case 6:
				s = "Literal Double expected";
				break;
			case 7:
				s = "Literal Single expected";
				break;
			case 8:
				s = "Literal Decimal expected";
				break;
			case 9:
				s = "Literal Date expected";
				break;
			case 10:
				s = "\".\" expected";
				break;
			case 11:
				s = "\"=\" expected";
				break;
			case 12:
				s = "\",\" expected";
				break;
			case 13:
				s = "\":\" expected";
				break;
			case 14:
				s = "\"+\" expected";
				break;
			case 15:
				s = "\"-\" expected";
				break;
			case 16:
				s = "\"*\" expected";
				break;
			case 17:
				s = "\"/\" expected";
				break;
			case 18:
				s = "\"\\\\\" expected";
				break;
			case 19:
				s = "\"&\" expected";
				break;
			case 20:
				s = "\"^\" expected";
				break;
			case 21:
				s = "\"?\" expected";
				break;
			case 22:
				s = "\"{\" expected";
				break;
			case 23:
				s = "\"}\" expected";
				break;
			case 24:
				s = "\"(\" expected";
				break;
			case 25:
				s = "\")\" expected";
				break;
			case 26:
				s = "\">\" expected";
				break;
			case 27:
				s = "\"<\" expected";
				break;
			case 28:
				s = "\"<>\" expected";
				break;
			case 29:
				s = "\">=\" expected";
				break;
			case 30:
				s = "\"<=\" expected";
				break;
			case 31:
				s = "\"<<\" expected";
				break;
			case 32:
				s = "\">>\" expected";
				break;
			case 33:
				s = "\"+=\" expected";
				break;
			case 34:
				s = "\"^=\" expected";
				break;
			case 35:
				s = "\"-=\" expected";
				break;
			case 36:
				s = "\"*=\" expected";
				break;
			case 37:
				s = "\"/=\" expected";
				break;
			case 38:
				s = "\"\\\\=\" expected";
				break;
			case 39:
				s = "\"<<=\" expected";
				break;
			case 40:
				s = "\">>=\" expected";
				break;
			case 41:
				s = "\"&=\" expected";
				break;
			case 42:
				s = "\"AddHandler\" expected";
				break;
			case 43:
				s = "\"AddressOf\" expected";
				break;
			case 44:
				s = "\"Alias\" expected";
				break;
			case 45:
				s = "\"And\" expected";
				break;
			case 46:
				s = "\"AndAlso\" expected";
				break;
			case 47:
				s = "\"Ansi\" expected";
				break;
			case 48:
				s = "\"As\" expected";
				break;
			case 49:
				s = "\"Assembly\" expected";
				break;
			case 50:
				s = "\"Auto\" expected";
				break;
			case 51:
				s = "\"Binary\" expected";
				break;
			case 52:
				s = "\"Boolean\" expected";
				break;
			case 53:
				s = "\"ByRef\" expected";
				break;
			case 54:
				s = "\"Byte\" expected";
				break;
			case 55:
				s = "\"ByVal\" expected";
				break;
			case 56:
				s = "\"Call\" expected";
				break;
			case 57:
				s = "\"Case\" expected";
				break;
			case 58:
				s = "\"Catch\" expected";
				break;
			case 59:
				s = "\"CBool\" expected";
				break;
			case 60:
				s = "\"CByte\" expected";
				break;
			case 61:
				s = "\"CChar\" expected";
				break;
			case 62:
				s = "\"CDate\" expected";
				break;
			case 63:
				s = "\"CDbl\" expected";
				break;
			case 64:
				s = "\"CDec\" expected";
				break;
			case 65:
				s = "\"Char\" expected";
				break;
			case 66:
				s = "\"CInt\" expected";
				break;
			case 67:
				s = "\"Class\" expected";
				break;
			case 68:
				s = "\"CLng\" expected";
				break;
			case 69:
				s = "\"CObj\" expected";
				break;
			case 70:
				s = "\"Compare\" expected";
				break;
			case 71:
				s = "\"Const\" expected";
				break;
			case 72:
				s = "\"CShort\" expected";
				break;
			case 73:
				s = "\"CSng\" expected";
				break;
			case 74:
				s = "\"CStr\" expected";
				break;
			case 75:
				s = "\"CType\" expected";
				break;
			case 76:
				s = "\"Date\" expected";
				break;
			case 77:
				s = "\"Decimal\" expected";
				break;
			case 78:
				s = "\"Declare\" expected";
				break;
			case 79:
				s = "\"Default\" expected";
				break;
			case 80:
				s = "\"Delegate\" expected";
				break;
			case 81:
				s = "\"Dim\" expected";
				break;
			case 82:
				s = "\"DirectCast\" expected";
				break;
			case 83:
				s = "\"Do\" expected";
				break;
			case 84:
				s = "\"Double\" expected";
				break;
			case 85:
				s = "\"Each\" expected";
				break;
			case 86:
				s = "\"Else\" expected";
				break;
			case 87:
				s = "\"ElseIf\" expected";
				break;
			case 88:
				s = "\"End\" expected";
				break;
			case 89:
				s = "\"EndIf\" expected";
				break;
			case 90:
				s = "\"Enum\" expected";
				break;
			case 91:
				s = "\"Erase\" expected";
				break;
			case 92:
				s = "\"Error\" expected";
				break;
			case 93:
				s = "\"Event\" expected";
				break;
			case 94:
				s = "\"Exit\" expected";
				break;
			case 95:
				s = "\"Explicit\" expected";
				break;
			case 96:
				s = "\"False\" expected";
				break;
			case 97:
				s = "\"Finally\" expected";
				break;
			case 98:
				s = "\"For\" expected";
				break;
			case 99:
				s = "\"Friend\" expected";
				break;
			case 100:
				s = "\"Function\" expected";
				break;
			case 101:
				s = "\"Get\" expected";
				break;
			case 102:
				s = "\"GetType\" expected";
				break;
			case 103:
				s = "\"GoSub\" expected";
				break;
			case 104:
				s = "\"GoTo\" expected";
				break;
			case 105:
				s = "\"Handles\" expected";
				break;
			case 106:
				s = "\"If\" expected";
				break;
			case 107:
				s = "\"Implements\" expected";
				break;
			case 108:
				s = "\"Imports\" expected";
				break;
			case 109:
				s = "\"In\" expected";
				break;
			case 110:
				s = "\"Inherits\" expected";
				break;
			case 111:
				s = "\"Integer\" expected";
				break;
			case 112:
				s = "\"Interface\" expected";
				break;
			case 113:
				s = "\"Is\" expected";
				break;
			case 114:
				s = "\"Let\" expected";
				break;
			case 115:
				s = "\"Lib\" expected";
				break;
			case 116:
				s = "\"Like\" expected";
				break;
			case 117:
				s = "\"Long\" expected";
				break;
			case 118:
				s = "\"Loop\" expected";
				break;
			case 119:
				s = "\"Me\" expected";
				break;
			case 120:
				s = "\"Mod\" expected";
				break;
			case 121:
				s = "\"Module\" expected";
				break;
			case 122:
				s = "\"MustInherit\" expected";
				break;
			case 123:
				s = "\"MustOverride\" expected";
				break;
			case 124:
				s = "\"MyBase\" expected";
				break;
			case 125:
				s = "\"MyClass\" expected";
				break;
			case 126:
				s = "\"Namespace\" expected";
				break;
			case 127:
				s = "\"New\" expected";
				break;
			case 128:
				s = "\"Next\" expected";
				break;
			case 129:
				s = "\"Not\" expected";
				break;
			case 130:
				s = "\"Nothing\" expected";
				break;
			case 131:
				s = "\"NotInheritable\" expected";
				break;
			case 132:
				s = "\"NotOverridable\" expected";
				break;
			case 133:
				s = "\"Object\" expected";
				break;
			case 134:
				s = "\"Off\" expected";
				break;
			case 135:
				s = "\"On\" expected";
				break;
			case 136:
				s = "\"Option\" expected";
				break;
			case 137:
				s = "\"Optional\" expected";
				break;
			case 138:
				s = "\"Or\" expected";
				break;
			case 139:
				s = "\"OrElse\" expected";
				break;
			case 140:
				s = "\"Overloads\" expected";
				break;
			case 141:
				s = "\"Overridable\" expected";
				break;
			case 142:
				s = "\"Overrides\" expected";
				break;
			case 143:
				s = "\"ParamArray\" expected";
				break;
			case 144:
				s = "\"Preserve\" expected";
				break;
			case 145:
				s = "\"Private\" expected";
				break;
			case 146:
				s = "\"Property\" expected";
				break;
			case 147:
				s = "\"Protected\" expected";
				break;
			case 148:
				s = "\"Public\" expected";
				break;
			case 149:
				s = "\"RaiseEvent\" expected";
				break;
			case 150:
				s = "\"ReadOnly\" expected";
				break;
			case 151:
				s = "\"ReDim\" expected";
				break;
			case 152:
				s = "\"RemoveHandler\" expected";
				break;
			case 153:
				s = "\"Resume\" expected";
				break;
			case 154:
				s = "\"Return\" expected";
				break;
			case 155:
				s = "\"Select\" expected";
				break;
			case 156:
				s = "\"Set\" expected";
				break;
			case 157:
				s = "\"Shadows\" expected";
				break;
			case 158:
				s = "\"Shared\" expected";
				break;
			case 159:
				s = "\"Short\" expected";
				break;
			case 160:
				s = "\"Single\" expected";
				break;
			case 161:
				s = "\"Static\" expected";
				break;
			case 162:
				s = "\"Step\" expected";
				break;
			case 163:
				s = "\"Stop\" expected";
				break;
			case 164:
				s = "\"Strict\" expected";
				break;
			case 165:
				s = "\"String\" expected";
				break;
			case 166:
				s = "\"Structure\" expected";
				break;
			case 167:
				s = "\"Sub\" expected";
				break;
			case 168:
				s = "\"SyncLock\" expected";
				break;
			case 169:
				s = "\"Text\" expected";
				break;
			case 170:
				s = "\"Then\" expected";
				break;
			case 171:
				s = "\"Throw\" expected";
				break;
			case 172:
				s = "\"To\" expected";
				break;
			case 173:
				s = "\"True\" expected";
				break;
			case 174:
				s = "\"Try\" expected";
				break;
			case 175:
				s = "\"TypeOf\" expected";
				break;
			case 176:
				s = "\"Unicode\" expected";
				break;
			case 177:
				s = "\"Until\" expected";
				break;
			case 178:
				s = "\"Variant\" expected";
				break;
			case 179:
				s = "\"Wend\" expected";
				break;
			case 180:
				s = "\"When\" expected";
				break;
			case 181:
				s = "\"While\" expected";
				break;
			case 182:
				s = "\"With\" expected";
				break;
			case 183:
				s = "\"WithEvents\" expected";
				break;
			case 184:
				s = "\"WriteOnly\" expected";
				break;
			case 185:
				s = "\"Xor\" expected";
				break;
			case 186:
				s = "\"Continue\" expected";
				break;
			case 187:
				s = "\"Operator\" expected";
				break;
			case 188:
				s = "\"Using\" expected";
				break;
			case 189:
				s = "\"IsNot\" expected";
				break;
			case 190:
				s = "\"SByte\" expected";
				break;
			case 191:
				s = "\"UInteger\" expected";
				break;
			case 192:
				s = "\"ULong\" expected";
				break;
			case 193:
				s = "\"UShort\" expected";
				break;
			case 194:
				s = "\"CSByte\" expected";
				break;
			case 195:
				s = "\"CUShort\" expected";
				break;
			case 196:
				s = "\"CUInt\" expected";
				break;
			case 197:
				s = "\"CULng\" expected";
				break;
			case 198:
				s = "\"Global\" expected";
				break;
			case 199:
				s = "\"TryCast\" expected";
				break;
			case 200:
				s = "\"Of\" expected";
				break;
			case 201:
				s = "\"Narrowing\" expected";
				break;
			case 202:
				s = "\"Widening\" expected";
				break;
			case 203:
				s = "\"Partial\" expected";
				break;
			case 204:
				s = "\"Custom\" expected";
				break;
			case 205:
				s = "??? expected";
				break;
			case 206:
				s = "invalid OptionStmt";
				break;
			case 207:
				s = "invalid OptionStmt";
				break;
			case 208:
				s = "invalid Global Attribute Section";
				break;
			case 209:
				s = "invalid Global Attribute Section";
				break;
			case 210:
				s = "invalid Namespace Member Declaration";
				break;
			case 211:
				s = "invalid Option Value";
				break;
			case 212:
				s = "invalid EndOfStmt";
				break;
			case 213:
				s = "invalid Type Modifier";
				break;
			case 214:
				s = "invalid Non Module Declaration";
				break;
			case 215:
				s = "invalid Non Module Declaration";
				break;
			case 216:
				s = "invalid Identifier";
				break;
			case 217:
				s = "invalid Type Parameter Constraints";
				break;
			case 218:
				s = "invalid Type Parameter Constraint";
				break;
			case 219:
				s = "invalid Non Array Type Name";
				break;
			case 220:
				s = "invalid Member Modifier";
				break;
			case 221:
				s = "invalid Structure Member Declaration";
				break;
			case 222:
				s = "invalid Structure Member Declaration";
				break;
			case 223:
				s = "invalid Structure Member Declaration";
				break;
			case 224:
				s = "invalid Structure Member Declaration";
				break;
			case 225:
				s = "invalid Structure Member Declaration";
				break;
			case 226:
				s = "invalid Structure Member Declaration";
				break;
			case 227:
				s = "invalid Structure Member Declaration";
				break;
			case 228:
				s = "invalid Interface Member Declaration";
				break;
			case 229:
				s = "invalid Interface Member Declaration";
				break;
			case 230:
				s = "invalid Charset";
				break;
			case 231:
				s = "invalid Identifier For Field Declaration";
				break;
			case 232:
				s = "invalid Variable Declarator Part After Identifier";
				break;
			case 233:
				s = "invalid Accessor Decls";
				break;
			case 234:
				s = "invalid Event Accessor Declaration";
				break;
			case 235:
				s = "invalid Overloadable Operator";
				break;
			case 236:
				s = "invalid Variable Initializer";
				break;
			case 237:
				s = "invalid Event Member Specifier";
				break;
			case 238:
				s = "invalid Assignment Operator";
				break;
			case 239:
				s = "invalid Simple Non Invocation Expression";
				break;
			case 240:
				s = "invalid Simple Non Invocation Expression";
				break;
			case 241:
				s = "invalid Simple Non Invocation Expression";
				break;
			case 242:
				s = "invalid Simple Non Invocation Expression";
				break;
			case 243:
				s = "invalid Invocation Expression";
				break;
			case 244:
				s = "invalid Invocation Expression";
				break;
			case 245:
				s = "invalid Primitive Type Name";
				break;
			case 246:
				s = "invalid Cast Target";
				break;
			case 247:
				s = "invalid Comparison Expression";
				break;
			case 248:
				s = "invalid Argument";
				break;
			case 249:
				s = "invalid QualIdent And Type Arguments";
				break;
			case 250:
				s = "invalid Attribute Arguments";
				break;
			case 251:
				s = "invalid Parameter Modifier";
				break;
			case 252:
				s = "invalid Statement";
				break;
			case 253:
				s = "invalid Label Name";
				break;
			case 254:
				s = "invalid Embedded Statement";
				break;
			case 255:
				s = "invalid Embedded Statement";
				break;
			case 256:
				s = "invalid Embedded Statement";
				break;
			case 257:
				s = "invalid Embedded Statement";
				break;
			case 258:
				s = "invalid Embedded Statement";
				break;
			case 259:
				s = "invalid Embedded Statement";
				break;
			case 260:
				s = "invalid Embedded Statement";
				break;
			case 261:
				s = "invalid While Or Until";
				break;
			case 262:
				s = "invalid Single Line Statement List";
				break;
			case 263:
				s = "invalid Single Line Statement List";
				break;
			case 264:
				s = "invalid On Error Statement";
				break;
			case 265:
				s = "invalid Resume Statement";
				break;
			case 266:
				s = "invalid Case Clause";
				break;
			case 267:
				s = "invalid Case Clause";
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
	}
}
