﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Pointer;

namespace Json.Schema
{
	/// <summary>
	/// Handles `dependentSchemas`.
	/// </summary>
	[Applicator]
	[SchemaPriority(10)]
	[SchemaKeyword(Name)]
	[SchemaDraft(Draft.Draft201909)]
	[Vocabulary(Vocabularies.Applicator201909Id)]
	[JsonConverter(typeof(DependentSchemasKeywordJsonConverter))]
	public class DependentSchemasKeyword : IJsonSchemaKeyword, IRefResolvable, IKeyedSchemaCollector, IEquatable<DependentSchemasKeyword>
	{
		internal const string Name = "dependentSchemas";

		/// <summary>
		/// The collection of "schema"-type dependencies.
		/// </summary>
		public IReadOnlyDictionary<string, JsonSchema> Schemas { get; }

		/// <summary>
		/// Creates a new <see cref="DependentSchemasKeyword"/>.
		/// </summary>
		/// <param name="values">The collection of "schema"-type dependencies.</param>
		public DependentSchemasKeyword(IReadOnlyDictionary<string, JsonSchema> values)
		{
			Schemas = values;
		}

		/// <summary>
		/// Provides validation for the keyword.
		/// </summary>
		/// <param name="context">Contextual details for the validation process.</param>
		public void Validate(ValidationContext context)
		{
			if (context.LocalInstance.ValueKind != JsonValueKind.Object)
			{
				context.IsValid = true;
				return;
			}

			var overallResult = true;
			var evaluatedProperties = new List<string>();
			foreach (var property in Schemas)
			{
				var schema = property.Value;
				var name = property.Key;
				if (!context.LocalInstance.TryGetProperty(name, out _)) continue;
				
				var subContext = ValidationContext.From(context,
					subschemaLocation: context.SchemaLocation.Combine(PointerSegment.Create($"{name}")));
				schema.ValidateSubschema(subContext);
				overallResult &= subContext.IsValid;
				if (!overallResult && context.ApplyOptimizations) break;
				context.NestedContexts.Add(subContext);
				if (subContext.IsValid)
					evaluatedProperties.Add(name);
			}

			context.ConsolidateAnnotations();
			context.IsValid = overallResult;
			if (!context.IsValid)
				context.Message = $"The following properties failed their dependent schemas: {JsonSerializer.Serialize(evaluatedProperties)}";
		}

		IRefResolvable IRefResolvable.ResolvePointerSegment(string value)
		{
			return Schemas.TryGetValue(value, out var schema) ? schema : null;
		}

		void IRefResolvable.RegisterSubschemas(SchemaRegistry registry, Uri currentUri)
		{
			foreach (var schema in Schemas.Values)
			{
				schema.RegisterSubschemas(registry, currentUri);
			}
		}

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
		public bool Equals(DependentSchemasKeyword other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			if (Schemas.Count != other.Schemas.Count) return false;
			var byKey = Schemas.Join(other.Schemas,
					td => td.Key,
					od => od.Key,
					(td, od) => new { ThisDef = td.Value, OtherDef = od.Value })
				.ToList();
			if (byKey.Count != Schemas.Count) return false;

			return byKey.All(g => Equals(g.ThisDef, g.OtherDef));
		}

		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as DependentSchemasKeyword);
		}

		/// <summary>Serves as the default hash function.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return Schemas.Aggregate(0, (current, obj) =>
			{
				unchecked
				{
					var hashCode = current;
					hashCode = (hashCode * 397) ^ (obj.Key?.GetHashCode() ?? 0);
					hashCode = (hashCode * 397) ^ (obj.Value != null ? obj.Value.GetHashCode() : 0);
					return hashCode;
				}
			});
		}
	}

	internal class DependentSchemasKeywordJsonConverter : JsonConverter<DependentSchemasKeyword>
	{
		public override DependentSchemasKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
				throw new JsonException("Expected object");

			var schema = JsonSerializer.Deserialize<Dictionary<string, JsonSchema>>(ref reader, options);
			return new DependentSchemasKeyword(schema);
		}
		public override void Write(Utf8JsonWriter writer, DependentSchemasKeyword value, JsonSerializerOptions options)
		{
			writer.WritePropertyName(DependentSchemasKeyword.Name);
			writer.WriteStartObject();
			foreach (var kvp in value.Schemas)
			{
				writer.WritePropertyName(kvp.Key);
				JsonSerializer.Serialize(writer, kvp.Value, options);
			}
			writer.WriteEndObject();
		}
	}
}