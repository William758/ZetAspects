using System.Collections;
using System.Collections.Generic;
using RoR2;
using RoR2.ContentManagement;

namespace TPDespair.ZetAspects
{
	public class ZetAspectsContent : IContentPackProvider
	{
		public ContentPack contentPack = new ContentPack();

		public string identifier
		{
			get { return "ZetAspectsContent"; }
		}

		public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
		{
			Catalog.OnContentLoadStart();

			contentPack.buffDefs.Add(buffDefs.ToArray());
			contentPack.itemDefs.Add(itemDefs.ToArray());

			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
		{
			ContentPack.Copy(contentPack, args.output);
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
		{
			args.ReportProgress(1f);
			yield break;
		}



		public static List<ItemDef> itemDefs = new List<ItemDef>();
		public static List<BuffDef> buffDefs = new List<BuffDef>();
	}
}
