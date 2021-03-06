﻿using System.Linq;
using System.Reflection;

namespace Json.Schema
{
	/// <summary>
	/// Declares the vocabularies of the supported drafts.
	/// </summary>
	public static class Vocabularies
	{
		/// <summary>
		/// The Draft 2019-09 Core vocabulary ID.
		/// </summary>
		public const string Core201909Id = "https://json-schema.org/draft/2019-09/vocab/core";
		/// <summary>
		/// The Draft 2019-09 Applicator vocabulary ID.
		/// </summary>
		public const string Applicator201909Id = "https://json-schema.org/draft/2019-09/vocab/applicator";
		/// <summary>
		/// The Draft 2019-09 Validation vocabulary ID.
		/// </summary>
		public const string Validation201909Id = "https://json-schema.org/draft/2019-09/vocab/validation";
		/// <summary>
		/// The Draft 2019-09 Metadata vocabulary ID.
		/// </summary>
		public const string Metadata201909Id = "https://json-schema.org/draft/2019-09/vocab/meta-data";
		/// <summary>
		/// The Draft 2019-09 Format vocabulary ID.
		/// </summary>
		public const string Format201909Id = "https://json-schema.org/draft/2019-09/vocab/format";
		/// <summary>
		/// The Draft 2019-09 Content vocabulary ID.
		/// </summary>
		public const string Content201909Id = "https://json-schema.org/draft/2019-09/vocab/content";

		/// <summary>
		/// The Draft 2019-09 Core vocabulary.
		/// </summary>
		public static readonly Vocabulary Core201909;
		/// <summary>
		/// The Draft 2019-09 Applicator vocabulary.
		/// </summary>
		public static readonly Vocabulary Applicator201909;
		/// <summary>
		/// The Draft 2019-09 Validation vocabulary.
		/// </summary>
		public static readonly Vocabulary Validation201909;
		/// <summary>
		/// The Draft 2019-09 Metadata vocabulary.
		/// </summary>
		public static readonly Vocabulary Metadata201909;
		/// <summary>
		/// The Draft 2019-09 Format vocabulary.
		/// </summary>
		public static readonly Vocabulary Format201909;
		/// <summary>
		/// The Draft 2019-09 Content vocabulary.
		/// </summary>
		public static readonly Vocabulary Content201909;

		static Vocabularies()
		{
			var keywords = typeof(IJsonSchemaKeyword)
				.Assembly
				.DefinedTypes
				.Where(t => typeof(IJsonSchemaKeyword).IsAssignableFrom(t) &&
				            !t.IsAbstract &&
				            !t.IsInterface)
				.Select(t => new
				{
					Type = t,
					Vocabularies = t.GetCustomAttributes<VocabularyAttribute>()
				})
				.ToList();
			Core201909 = new Vocabulary(
				Core201909Id,
				keywords.Where(k => k.Vocabularies.Any(v => v.Id.OriginalString == Core201909Id))
					.Select(k => k.Type));
			Applicator201909 = new Vocabulary(
				Applicator201909Id,
				keywords.Where(k => k.Vocabularies.Any(v => v.Id.OriginalString == Applicator201909Id))
					.Select(k => k.Type));
			Validation201909 = new Vocabulary(
				Validation201909Id,
				keywords.Where(k => k.Vocabularies.Any(v => v.Id.OriginalString == Validation201909Id))
					.Select(k => k.Type));
			Metadata201909 = new Vocabulary(
				Metadata201909Id,
				keywords.Where(k => k.Vocabularies.Any(v => v.Id.OriginalString == Metadata201909Id))
					.Select(k => k.Type));
			Format201909 = new Vocabulary(
				Format201909Id,
				keywords.Where(k => k.Vocabularies.Any(v => v.Id.OriginalString == Format201909Id))
					.Select(k => k.Type));
			Content201909 = new Vocabulary(
				Content201909Id,
				keywords.Where(k => k.Vocabularies.Any(v => v.Id.OriginalString == Content201909Id))
					.Select(k => k.Type));
		}
	}
}