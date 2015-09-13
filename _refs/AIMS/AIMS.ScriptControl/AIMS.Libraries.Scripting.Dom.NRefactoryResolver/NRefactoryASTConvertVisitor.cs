using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Visitors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom.NRefactoryResolver
{
	public class NRefactoryASTConvertVisitor : AbstractAstVisitor
	{
		private ICompilationUnit cu;

		private Stack<string> currentNamespace = new Stack<string>();

		private Stack<DefaultClass> currentClass = new Stack<DefaultClass>();

		private List<ISpecial> specials;

		public ICompilationUnit Cu
		{
			get
			{
				return this.cu;
			}
		}

		public List<ISpecial> Specials
		{
			get
			{
				return this.specials;
			}
			set
			{
				this.specials = value;
			}
		}

		private bool IsVisualBasic
		{
			get
			{
				return this.cu.ProjectContent.Language == LanguageProperties.VBNet;
			}
		}

		public NRefactoryASTConvertVisitor(IProjectContent projectContent)
		{
			this.cu = new DefaultCompilationUnit(projectContent);
		}

		private DefaultClass GetCurrentClass()
		{
			return (this.currentClass.Count == 0) ? null : this.currentClass.Peek();
		}

		private ModifierEnum ConvertModifier(Modifiers m)
		{
			ModifierEnum result;
			if (this.IsVisualBasic)
			{
				result = this.ConvertModifier(m, ModifierEnum.Public);
			}
			else if (this.currentClass.Count > 0 && this.currentClass.Peek().ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Interface)
			{
				result = this.ConvertModifier(m, ModifierEnum.Public);
			}
			else
			{
				result = this.ConvertModifier(m, ModifierEnum.Private);
			}
			return result;
		}

		private ModifierEnum ConvertTypeModifier(Modifiers m)
		{
			ModifierEnum result;
			if (this.IsVisualBasic)
			{
				result = this.ConvertModifier(m, ModifierEnum.Public);
			}
			else if (this.currentClass.Count > 0)
			{
				result = this.ConvertModifier(m, ModifierEnum.Private);
			}
			else
			{
				result = this.ConvertModifier(m, ModifierEnum.Internal);
			}
			return result;
		}

		private ModifierEnum ConvertModifier(Modifiers m, ModifierEnum defaultVisibility)
		{
			ModifierEnum result;
			if ((m & Modifiers.Visibility) == Modifiers.None)
			{
				result = (ModifierEnum)(m | (Modifiers)defaultVisibility);
			}
			else
			{
				result = (ModifierEnum)m;
			}
			return result;
		}

		private string GetDocumentation(int line, IList<AttributeSection> attributes)
		{
			foreach (AttributeSection att in attributes)
			{
				if (att.StartLocation.Y > 0 && att.StartLocation.Y < line)
				{
					line = att.StartLocation.Y;
				}
			}
			List<string> lines = new List<string>();
			int length = 0;
			while (line > 0)
			{
				string doku = this.GetDocumentationFromLine(--line);
				if (doku == null)
				{
					break;
				}
				length += 2 + doku.Length;
				lines.Add(doku);
			}
			StringBuilder b = new StringBuilder(length);
			for (int i = lines.Count - 1; i >= 0; i--)
			{
				b.AppendLine(lines[i]);
			}
			return b.ToString();
		}

		private string GetDocumentationFromLine(int line)
		{
			string result;
			if (this.specials == null)
			{
				result = null;
			}
			else if (line < 0)
			{
				result = null;
			}
			else
			{
				int left = 0;
				int right = this.specials.Count - 1;
				while (left <= right)
				{
					int leftLine = this.specials[left].StartPosition.Y;
					if (line < leftLine)
					{
						break;
					}
					int rightLine = this.specials[right].StartPosition.Y;
					if (line > rightLine)
					{
						break;
					}
					int i;
					if (leftLine == rightLine)
					{
						if (leftLine != line)
						{
							break;
						}
						i = left;
					}
					else
					{
						i = (int)((long)left + Math.BigMul(line - leftLine, right - left) / (long)(rightLine - leftLine));
					}
					int mLine = this.specials[i].StartPosition.Y;
					if (mLine < line)
					{
						left = i + 1;
					}
					else
					{
						if (mLine <= line)
						{
							while (--i >= 0 && this.specials[i].StartPosition.Y == line)
							{
							}
							while (++i < this.specials.Count && this.specials[i].StartPosition.Y == line)
							{
								Comment comment = this.specials[i] as Comment;
								if (comment != null && comment.CommentType == CommentType.Documentation)
								{
									result = comment.CommentText;
									return result;
								}
							}
							break;
						}
						right = i - 1;
					}
				}
				result = null;
			}
			return result;
		}

		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			object result;
			if (compilationUnit == null)
			{
				result = null;
			}
			else
			{
				compilationUnit.AcceptChildren(this, data);
				result = this.cu;
			}
			return result;
		}

		public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
		{
			DefaultUsing us = new DefaultUsing(this.cu.ProjectContent, NRefactoryASTConvertVisitor.GetRegion(usingDeclaration.StartLocation, usingDeclaration.EndLocation));
			foreach (Using u in usingDeclaration.Usings)
			{
				u.AcceptVisitor(this, us);
			}
			this.cu.Usings.Add(us);
			return data;
		}

		public override object VisitUsing(Using u, object data)
		{
			Debug.Assert(data is DefaultUsing);
			DefaultUsing us = (DefaultUsing)data;
			if (u.IsAlias)
			{
				IReturnType rt = this.CreateReturnType(u.Alias);
				if (rt != null)
				{
					us.AddAlias(u.Name, rt);
				}
			}
			else
			{
				us.Usings.Add(u.Name);
			}
			return data;
		}

		private void ConvertAttributes(AttributedNode from, AbstractDecoration to)
		{
			if (from.Attributes.Count == 0)
			{
				to.Attributes = DefaultAttribute.EmptyAttributeList;
			}
			else
			{
				to.Attributes = this.VisitAttributes(from.Attributes);
			}
		}

		private List<IAttribute> VisitAttributes(List<AttributeSection> attributes)
		{
			List<IAttribute> result = new List<IAttribute>();
			foreach (AttributeSection section in attributes)
			{
				AttributeTarget target = AttributeTarget.None;
				if (section.AttributeTarget != null && section.AttributeTarget != "")
				{
					string text = section.AttributeTarget.ToUpperInvariant();
					if (text == null)
					{
						goto IL_143;
					}
					if (__Class1.__member1 == null)
					{
						__Class1.__member1 = new Dictionary<string, int>(9)
						{
							{
								"ASSEMBLY",
								0
							},
							{
								"FIELD",
								1
							},
							{
								"EVENT",
								2
							},
							{
								"METHOD",
								3
							},
							{
								"MODULE",
								4
							},
							{
								"PARAM",
								5
							},
							{
								"PROPERTY",
								6
							},
							{
								"RETURN",
								7
							},
							{
								"TYPE",
								8
							}
						};
					}
					int num;
					if (!__Class1.__member1.TryGetValue(text, out num))
					{
						goto IL_143;
					}
					switch (num)
					{
					case 0:
						target = AttributeTarget.Assembly;
						break;
					case 1:
						target = AttributeTarget.Field;
						break;
					case 2:
						target = AttributeTarget.Event;
						break;
					case 3:
						target = AttributeTarget.Method;
						break;
					case 4:
						target = AttributeTarget.Module;
						break;
					case 5:
						target = AttributeTarget.Param;
						break;
					case 6:
						target = AttributeTarget.Property;
						break;
					case 7:
						target = AttributeTarget.Return;
						break;
					case 8:
						target = AttributeTarget.Type;
						break;
					default:
						goto IL_143;
					}
					goto IL_148;
					IL_143:
					target = AttributeTarget.None;
				}
				IL_148:
				foreach (AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute in section.Attributes)
				{
					result.Add(new DefaultAttribute(attribute.Name, target));
				}
			}
			return result;
		}

		public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			string name;
			if (this.currentNamespace.Count == 0)
			{
				name = namespaceDeclaration.Name;
			}
			else
			{
				name = this.currentNamespace.Peek() + '.' + namespaceDeclaration.Name;
			}
			this.currentNamespace.Push(name);
			object ret = namespaceDeclaration.AcceptChildren(this, data);
			this.currentNamespace.Pop();
			return ret;
		}

		private AIMS.Libraries.Scripting.Dom.ClassType TranslateClassType(AIMS.Libraries.Scripting.NRefactory.Ast.ClassType type)
		{
			AIMS.Libraries.Scripting.Dom.ClassType result;
			switch (type)
			{
			case AIMS.Libraries.Scripting.NRefactory.Ast.ClassType.Module:
				result = AIMS.Libraries.Scripting.Dom.ClassType.Module;
				break;
			case AIMS.Libraries.Scripting.NRefactory.Ast.ClassType.Interface:
				result = AIMS.Libraries.Scripting.Dom.ClassType.Interface;
				break;
			case AIMS.Libraries.Scripting.NRefactory.Ast.ClassType.Struct:
				result = AIMS.Libraries.Scripting.Dom.ClassType.Struct;
				break;
			case AIMS.Libraries.Scripting.NRefactory.Ast.ClassType.Enum:
				result = AIMS.Libraries.Scripting.Dom.ClassType.Enum;
				break;
			default:
				result = AIMS.Libraries.Scripting.Dom.ClassType.Class;
				break;
			}
			return result;
		}

		private static DomRegion GetRegion(Location start, Location end)
		{
			return new DomRegion(start, end);
		}

		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(typeDeclaration.StartLocation, typeDeclaration.EndLocation);
			DomRegion bodyRegion = NRefactoryASTConvertVisitor.GetRegion(typeDeclaration.BodyStartLocation, typeDeclaration.EndLocation);
			DefaultClass c = new DefaultClass(this.cu, this.TranslateClassType(typeDeclaration.Type), this.ConvertTypeModifier(typeDeclaration.Modifier), region, this.GetCurrentClass());
			c.BodyRegion = bodyRegion;
			this.ConvertAttributes(typeDeclaration, c);
			c.Documentation = this.GetDocumentation(region.BeginLine, typeDeclaration.Attributes);
			if (this.currentClass.Count > 0)
			{
				DefaultClass cur = this.GetCurrentClass();
				cur.InnerClasses.Add(c);
				c.FullyQualifiedName = cur.FullyQualifiedName + '.' + typeDeclaration.Name;
			}
			else
			{
				if (this.currentNamespace.Count == 0)
				{
					c.FullyQualifiedName = typeDeclaration.Name;
				}
				else
				{
					c.FullyQualifiedName = this.currentNamespace.Peek() + '.' + typeDeclaration.Name;
				}
				this.cu.Classes.Add(c);
			}
			this.currentClass.Push(c);
			if (c.ClassType != AIMS.Libraries.Scripting.Dom.ClassType.Enum && typeDeclaration.BaseTypes != null)
			{
				foreach (TypeReference type in typeDeclaration.BaseTypes)
				{
					IReturnType rt = this.CreateReturnType(type);
					if (rt != null)
					{
						c.BaseTypes.Add(rt);
					}
				}
			}
			this.ConvertTemplates(typeDeclaration.Templates, c);
			object ret = typeDeclaration.AcceptChildren(this, data);
			this.currentClass.Pop();
			if (c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Module)
			{
				foreach (IField f in c.Fields)
				{
					f.Modifiers |= ModifierEnum.Static;
				}
				foreach (IMethod i in c.Methods)
				{
					i.Modifiers |= ModifierEnum.Static;
				}
				foreach (IProperty p in c.Properties)
				{
					p.Modifiers |= ModifierEnum.Static;
				}
				foreach (IEvent e in c.Events)
				{
					e.Modifiers |= ModifierEnum.Static;
				}
			}
			return ret;
		}

		private void ConvertTemplates(IList<TemplateDefinition> templateList, DefaultClass c)
		{
			int index = 0;
			if (templateList.Count == 0)
			{
				c.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
			}
			else
			{
				foreach (TemplateDefinition template in templateList)
				{
					c.TypeParameters.Add(this.ConvertConstraints(template, new DefaultTypeParameter(c, template.Name, index++)));
				}
			}
		}

		private void ConvertTemplates(List<TemplateDefinition> templateList, DefaultMethod m)
		{
			int index = 0;
			if (templateList.Count == 0)
			{
				m.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
			}
			else
			{
				foreach (TemplateDefinition template in templateList)
				{
					m.TypeParameters.Add(this.ConvertConstraints(template, new DefaultTypeParameter(m, template.Name, index++)));
				}
			}
		}

		private DefaultTypeParameter ConvertConstraints(TemplateDefinition template, DefaultTypeParameter typeParameter)
		{
			foreach (TypeReference typeRef in template.Bases)
			{
				if (typeRef == TypeReference.NewConstraint)
				{
					typeParameter.HasConstructableConstraint = true;
				}
				else if (typeRef == TypeReference.ClassConstraint)
				{
					typeParameter.HasReferenceTypeConstraint = true;
				}
				else if (typeRef == TypeReference.StructConstraint)
				{
					typeParameter.HasValueTypeConstraint = true;
				}
				else
				{
					IReturnType rt = this.CreateReturnType(typeRef);
					if (rt != null)
					{
						typeParameter.Constraints.Add(rt);
					}
				}
			}
			return typeParameter;
		}

		public override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(delegateDeclaration.StartLocation, delegateDeclaration.EndLocation);
			DefaultClass c = new DefaultClass(this.cu, AIMS.Libraries.Scripting.Dom.ClassType.Delegate, this.ConvertTypeModifier(delegateDeclaration.Modifier), region, this.GetCurrentClass());
			c.Documentation = this.GetDocumentation(region.BeginLine, delegateDeclaration.Attributes);
			this.ConvertAttributes(delegateDeclaration, c);
			this.CreateDelegate(c, delegateDeclaration.Name, delegateDeclaration.ReturnType, delegateDeclaration.Templates, delegateDeclaration.Parameters);
			return c;
		}

		private void CreateDelegate(DefaultClass c, string name, TypeReference returnType, IList<TemplateDefinition> templates, IList<ParameterDeclarationExpression> parameters)
		{
			c.BaseTypes.Add(c.ProjectContent.SystemTypes.Delegate);
			if (this.currentClass.Count > 0)
			{
				DefaultClass cur = this.GetCurrentClass();
				cur.InnerClasses.Add(c);
				c.FullyQualifiedName = cur.FullyQualifiedName + '.' + name;
			}
			else
			{
				if (this.currentNamespace.Count == 0)
				{
					c.FullyQualifiedName = name;
				}
				else
				{
					c.FullyQualifiedName = this.currentNamespace.Peek() + '.' + name;
				}
				this.cu.Classes.Add(c);
			}
			this.currentClass.Push(c);
			this.ConvertTemplates(templates, c);
			List<IParameter> p = new List<IParameter>();
			if (parameters != null)
			{
				foreach (ParameterDeclarationExpression param in parameters)
				{
					p.Add(this.CreateParameter(param));
				}
			}
			AnonymousMethodReturnType.AddDefaultDelegateMethod(c, this.CreateReturnType(returnType), p);
			this.currentClass.Pop();
		}

		private IParameter CreateParameter(ParameterDeclarationExpression par)
		{
			return this.CreateParameter(par, null);
		}

		private IParameter CreateParameter(ParameterDeclarationExpression par, IMethod method)
		{
			return NRefactoryASTConvertVisitor.CreateParameter(par, method, this.GetCurrentClass(), this.cu);
		}

		internal static IParameter CreateParameter(ParameterDeclarationExpression par, IMethod method, IClass currentClass, ICompilationUnit cu)
		{
			IReturnType parType = NRefactoryASTConvertVisitor.CreateReturnType(par.TypeReference, method, currentClass, cu);
			return new DefaultParameter(par.ParameterName, parType, NRefactoryASTConvertVisitor.GetRegion(par.StartLocation, par.EndLocation))
			{
				Modifiers = (AIMS.Libraries.Scripting.Dom.ParameterModifiers)par.ParamModifier
			};
		}

		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(methodDeclaration.StartLocation, methodDeclaration.EndLocation);
			DomRegion bodyRegion = NRefactoryASTConvertVisitor.GetRegion(methodDeclaration.EndLocation, (methodDeclaration.Body != null) ? methodDeclaration.Body.EndLocation : Location.Empty);
			DefaultClass c = this.GetCurrentClass();
			DefaultMethod method = new DefaultMethod(methodDeclaration.Name, null, this.ConvertModifier(methodDeclaration.Modifier), region, bodyRegion, this.GetCurrentClass());
			method.Documentation = this.GetDocumentation(region.BeginLine, methodDeclaration.Attributes);
			this.ConvertTemplates(methodDeclaration.Templates, method);
			method.ReturnType = this.CreateReturnType(methodDeclaration.TypeReference, method);
			this.ConvertAttributes(methodDeclaration, method);
			if (methodDeclaration.Parameters != null && methodDeclaration.Parameters.Count > 0)
			{
				foreach (ParameterDeclarationExpression par in methodDeclaration.Parameters)
				{
					method.Parameters.Add(this.CreateParameter(par, method));
				}
			}
			else
			{
				method.Parameters = DefaultParameter.EmptyParameterList;
			}
			c.Methods.Add(method);
			return null;
		}

		public override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			DefaultClass c = this.GetCurrentClass();
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(operatorDeclaration.StartLocation, operatorDeclaration.EndLocation);
			DomRegion bodyRegion = NRefactoryASTConvertVisitor.GetRegion(operatorDeclaration.EndLocation, (operatorDeclaration.Body != null) ? operatorDeclaration.Body.EndLocation : Location.Empty);
			DefaultMethod method = new DefaultMethod(operatorDeclaration.Name, this.CreateReturnType(operatorDeclaration.TypeReference), this.ConvertModifier(operatorDeclaration.Modifier), region, bodyRegion, c);
			this.ConvertAttributes(operatorDeclaration, method);
			if (operatorDeclaration.Parameters != null)
			{
				foreach (ParameterDeclarationExpression par in operatorDeclaration.Parameters)
				{
					method.Parameters.Add(this.CreateParameter(par, method));
				}
			}
			c.Methods.Add(method);
			return null;
		}

		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(constructorDeclaration.StartLocation, constructorDeclaration.EndLocation);
			DomRegion bodyRegion = NRefactoryASTConvertVisitor.GetRegion(constructorDeclaration.EndLocation, (constructorDeclaration.Body != null) ? constructorDeclaration.Body.EndLocation : Location.Empty);
			DefaultClass c = this.GetCurrentClass();
			Constructor constructor = new Constructor(this.ConvertModifier(constructorDeclaration.Modifier), region, bodyRegion, this.GetCurrentClass());
			constructor.Documentation = this.GetDocumentation(region.BeginLine, constructorDeclaration.Attributes);
			this.ConvertAttributes(constructorDeclaration, constructor);
			if (constructorDeclaration.Parameters != null)
			{
				foreach (ParameterDeclarationExpression par in constructorDeclaration.Parameters)
				{
					constructor.Parameters.Add(this.CreateParameter(par));
				}
			}
			c.Methods.Add(constructor);
			return null;
		}

		public override object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(destructorDeclaration.StartLocation, destructorDeclaration.EndLocation);
			DomRegion bodyRegion = NRefactoryASTConvertVisitor.GetRegion(destructorDeclaration.EndLocation, (destructorDeclaration.Body != null) ? destructorDeclaration.Body.EndLocation : Location.Empty);
			DefaultClass c = this.GetCurrentClass();
			Destructor destructor = new Destructor(region, bodyRegion, c);
			this.ConvertAttributes(destructorDeclaration, destructor);
			c.Methods.Add(destructor);
			return null;
		}

		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(fieldDeclaration.StartLocation, fieldDeclaration.EndLocation);
			DefaultClass c = this.GetCurrentClass();
			ModifierEnum modifier = this.ConvertModifier(fieldDeclaration.Modifier, (c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Struct && this.IsVisualBasic) ? ModifierEnum.Public : ModifierEnum.Private);
			string doku = this.GetDocumentation(region.BeginLine, fieldDeclaration.Attributes);
			if (this.currentClass.Count > 0)
			{
				for (int i = 0; i < fieldDeclaration.Fields.Count; i++)
				{
					VariableDeclaration field = fieldDeclaration.Fields[i];
					IReturnType retType;
					if (c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Enum)
					{
						retType = c.DefaultReturnType;
					}
					else
					{
						retType = this.CreateReturnType(fieldDeclaration.GetTypeForField(i));
						if (!field.FixedArrayInitialization.IsNull)
						{
							retType = new ArrayReturnType(this.cu.ProjectContent, retType, 1);
						}
					}
					DefaultField f = new DefaultField(retType, field.Name, modifier, region, c);
					this.ConvertAttributes(fieldDeclaration, f);
					f.Documentation = doku;
					if (c.ClassType == AIMS.Libraries.Scripting.Dom.ClassType.Enum)
					{
						f.Modifiers = (ModifierEnum.Public | ModifierEnum.Const);
					}
					c.Fields.Add(f);
				}
			}
			return null;
		}

		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(propertyDeclaration.StartLocation, propertyDeclaration.EndLocation);
			DomRegion bodyRegion = NRefactoryASTConvertVisitor.GetRegion(propertyDeclaration.BodyStart, propertyDeclaration.BodyEnd);
			IReturnType type = this.CreateReturnType(propertyDeclaration.TypeReference);
			DefaultClass c = this.GetCurrentClass();
			DefaultProperty property = new DefaultProperty(propertyDeclaration.Name, type, this.ConvertModifier(propertyDeclaration.Modifier), region, bodyRegion, this.GetCurrentClass());
			if (propertyDeclaration.HasGetRegion)
			{
				property.GetterRegion = NRefactoryASTConvertVisitor.GetRegion(propertyDeclaration.GetRegion.StartLocation, propertyDeclaration.GetRegion.EndLocation);
				property.CanGet = true;
			}
			if (propertyDeclaration.HasSetRegion)
			{
				property.SetterRegion = NRefactoryASTConvertVisitor.GetRegion(propertyDeclaration.SetRegion.StartLocation, propertyDeclaration.SetRegion.EndLocation);
				property.CanSet = true;
			}
			property.Documentation = this.GetDocumentation(region.BeginLine, propertyDeclaration.Attributes);
			this.ConvertAttributes(propertyDeclaration, property);
			c.Properties.Add(property);
			return null;
		}

		public override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(eventDeclaration.StartLocation, eventDeclaration.EndLocation);
			DomRegion bodyRegion = NRefactoryASTConvertVisitor.GetRegion(eventDeclaration.BodyStart, eventDeclaration.BodyEnd);
			DefaultClass c = this.GetCurrentClass();
			IReturnType type;
			if (eventDeclaration.TypeReference.IsNull)
			{
				DefaultClass del = new DefaultClass(this.cu, AIMS.Libraries.Scripting.Dom.ClassType.Delegate, this.ConvertModifier(eventDeclaration.Modifier), region, c);
				del.Modifiers |= ModifierEnum.Synthetic;
				this.CreateDelegate(del, eventDeclaration.Name + "EventHandler", new TypeReference("System.Void"), new TemplateDefinition[0], eventDeclaration.Parameters);
				type = del.DefaultReturnType;
			}
			else
			{
				type = this.CreateReturnType(eventDeclaration.TypeReference);
			}
			DefaultEvent e = new DefaultEvent(eventDeclaration.Name, type, this.ConvertModifier(eventDeclaration.Modifier), region, bodyRegion, c);
			this.ConvertAttributes(eventDeclaration, e);
			c.Events.Add(e);
			if (e != null)
			{
				e.Documentation = this.GetDocumentation(region.BeginLine, eventDeclaration.Attributes);
			}
			else
			{
				LoggingService.Warn("NRefactoryASTConvertVisitor: " + eventDeclaration + " has no events!");
			}
			return null;
		}

		public override object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			DomRegion region = NRefactoryASTConvertVisitor.GetRegion(indexerDeclaration.StartLocation, indexerDeclaration.EndLocation);
			DomRegion bodyRegion = NRefactoryASTConvertVisitor.GetRegion(indexerDeclaration.BodyStart, indexerDeclaration.BodyEnd);
			DefaultProperty i = new DefaultProperty("Indexer", this.CreateReturnType(indexerDeclaration.TypeReference), this.ConvertModifier(indexerDeclaration.Modifier), region, bodyRegion, this.GetCurrentClass());
			i.IsIndexer = true;
			i.Documentation = this.GetDocumentation(region.BeginLine, indexerDeclaration.Attributes);
			this.ConvertAttributes(indexerDeclaration, i);
			if (indexerDeclaration.Parameters != null)
			{
				foreach (ParameterDeclarationExpression par in indexerDeclaration.Parameters)
				{
					i.Parameters.Add(this.CreateParameter(par));
				}
			}
			DefaultClass c = this.GetCurrentClass();
			c.Properties.Add(i);
			return null;
		}

		private IReturnType CreateReturnType(TypeReference reference, IMethod method)
		{
			return NRefactoryASTConvertVisitor.CreateReturnType(reference, method, this.GetCurrentClass(), this.cu);
		}

		private static IReturnType CreateReturnType(TypeReference reference, IMethod method, IClass currentClass, ICompilationUnit cu)
		{
			IReturnType result;
			if (currentClass == null)
			{
				result = TypeVisitor.CreateReturnType(reference, new DefaultClass(cu, "___DummyClass"), method, 1, 1, cu.ProjectContent, true);
			}
			else
			{
				result = TypeVisitor.CreateReturnType(reference, currentClass, method, currentClass.Region.BeginLine + 1, 1, cu.ProjectContent, true);
			}
			return result;
		}

		private IReturnType CreateReturnType(TypeReference reference)
		{
			return this.CreateReturnType(reference, null);
		}
	}
}
