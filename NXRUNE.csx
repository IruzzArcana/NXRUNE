using System;
using System.Collections.Generic;
using System.IO;
using UndertaleModLib.Util;

EnsureDataLoaded();

if (Data?.GeneralInfo?.DisplayName?.Content.ToLower() != "deltarune")
{
    ScriptError("Error : Not a Deltarune data.win file");
    return;
}

var decompSettings = new Underanalyzer.Decompiler.DecompileSettings()
    {
        RemoveSingleLineBlockBraces = true,
        EmptyLineAroundBranchStatements = true,
        EmptyLineBeforeSwitchCases = true,
    };

UndertaleModLib.Compiler.CodeImportGroup importGroup = new(Data, null, decompSettings)
{
    ThrowOnNoOpFindReplace = true
};

// gml_Object_obj_init_pc

importGroup.QueueFindReplace("gml_Object_obj_init_pc_Create_0", "if (display_width > (640 * _ww) && display_height > (480 * _ww))", "if (display_width > (640 * _ww) && display_height > (360 * _ww))");

importGroup.QueueFindReplace("gml_Object_obj_init_pc_Create_0", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");

importGroup.QueueFindReplace("gml_Object_obj_init_pc_Draw_77", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");

importGroup.QueueAppend("gml_Object_obj_init_pc_Draw_77", @"var ww = window_get_width();
var wh = window_get_height();
var border_w = 1920;
var border_h = 1080;

var xx, yy;

if ((ww / wh) > (border_w / border_h))
{
    var scale = wh / border_h;
    border_w *= scale;
    border_h *= scale;
    xx = (320 * (wh / 1080)) + (abs(ww - border_w) / 2);
    yy = 60 * (wh / 1080);
}
else
{
    var scale = ww / border_w;
    border_w *= scale;
    border_h *= scale;
    xx = 320 * (ww / 1920);
    yy = (60 * (ww / 1920)) + (abs(wh - border_h) / 2);
}

texture_set_interpolation(false);
draw_enable_alphablend(false);
draw_surface_stretched(application_surface, xx, yy, ww - (2 * xx), wh - (2 * yy));
draw_enable_alphablend(true);
texture_set_interpolation(false);
");

// gml_Object_obj_screen_transition

importGroup.QueueFindReplace("gml_Object_obj_screen_transition_Step_0", "if (global.is_console)", "    if (true)");

importGroup.Import();

ScriptMessage("All done! :3");