
using Godot;

namespace StS2Hoshino.StS2HoshinoCode.UI;

[GlobalClass]
public partial class HoshinoNRestSiteCharacter : MegaCrit.Sts2.Core.Nodes.RestSite.NRestSiteCharacter
{
	public override void _Ready()
	{
		StS2HoshinoMain.Logger.Info("[HoshinoNRestSiteCharacter] _Ready called successfully.");
		base._Ready();
	}
}
