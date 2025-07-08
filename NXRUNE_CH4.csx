using System;
using System.Collections.Generic;
using System.IO;
using UndertaleModLib.Util;

EnsureDataLoaded();

if (Data?.GeneralInfo?.DisplayName?.Content.ToLower() != "deltarune chapter 4")
{
    ScriptError("Error : Not a Deltarune CH4 data.win file");
    return;
}

string bordersPath = Path.Combine(Path.GetDirectoryName(ScriptPath), "Borders/chapter4");

Dictionary<string, UndertaleEmbeddedTexture> textures = new();
if (!Directory.Exists(bordersPath))
{
    throw new ScriptException("Border textures not found??");
}

int lastTextPage = Data.EmbeddedTextures.Count - 1;
int lastTextPageItem = Data.TexturePageItems.Count - 1;

foreach (var path in Directory.EnumerateFiles(bordersPath))
{
    UndertaleEmbeddedTexture newtex = new UndertaleEmbeddedTexture();
    newtex.Name = new UndertaleString($"Texture {++lastTextPage}");
    newtex.TextureData.Image = GMImage.FromPng(File.ReadAllBytes(path));
    Data.EmbeddedTextures.Add(newtex);
    textures.Add(Path.GetFileName(path), newtex);
}

Action<string, UndertaleEmbeddedTexture, ushort, ushort, ushort, ushort, ushort, ushort, ushort, ushort> AssignBorderBackground = (name, tex, x, y, width, height, tarX, tarY, tarWidth, tarHeight) =>
{
    var bg = Data.Sprites.ByName(name);
    if (bg is null)
    {
        ScriptError(name + " not found!");
        return;
    }
    UndertaleTexturePageItem tpag = new UndertaleTexturePageItem();
    tpag.Name = new UndertaleString($"PageItem {++lastTextPageItem}");
    tpag.SourceX = x; tpag.SourceY = y; tpag.SourceWidth = width; tpag.SourceHeight = height;
    tpag.TargetX = tarX; tpag.TargetY = tarY; tpag.TargetWidth = tarWidth; tpag.TargetHeight = tarHeight;
    tpag.BoundingWidth = width; tpag.BoundingHeight = height;
    tpag.TexturePage = tex;
    Data.TexturePageItems.Add(tpag);
    bg.Textures[0].Texture = tpag;
};

AssignBorderBackground("border_dw_titan_base", textures["border_dw_titan_base.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
AssignBorderBackground("border_dw_titan_eyes_red", textures["border_dw_castletown.png"], 2, 1086, 1920, 941, 0, 70, 1920, 941);
AssignBorderBackground("border_lw_town_night", textures["border_lw_town_night.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
AssignBorderBackground("border_dw_titan_eyes", textures["border_dw_church_a.png"], 2, 1086, 1920, 881, 0, 100, 1920, 881);
AssignBorderBackground("border_line_1080", textures["border_line_1080.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
AssignBorderBackground("border_lw_town", textures["border_lw_town.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
AssignBorderBackground("border_dw_castletown", textures["border_dw_castletown.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
AssignBorderBackground("border_dw_church_c", textures["border_dw_church_c.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
AssignBorderBackground("border_dw_church_a", textures["border_dw_church_a.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);
AssignBorderBackground("border_dw_church_b", textures["border_dw_church_b.png"], 2, 2, 1920, 1080, 0, 0, 1920, 1080);



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

// obj_initializer2

importGroup.QueueFindReplace("gml_Object_obj_initializer2_Create_0", @"if (global.is_console)
    loadtex = instance_create(0, 0, obj_prefetchtex);", @"if (true)
    loadtex = instance_create(0, 0, obj_prefetchtex);");

importGroup.QueueFindReplace("gml_Object_obj_initializer2_Step_0", @"    if (!textures_loaded)
        textures_loaded = loadtex.loaded;
    
    if (textures_loaded)
        show_debug_message_concat(""TEXTURES LOADED"");
    else
        exit;
}", @"}
    if (!textures_loaded)
        textures_loaded = loadtex.loaded;
    
    if (textures_loaded)
        show_debug_message_concat(""TEXTURES LOADED"");
    else
        exit;");

importGroup.QueueFindReplace("gml_Object_obj_initializer2_Step_0", @"        if (global.is_console)
            global.screen_border_alpha = 0;", "global.screen_border_alpha = 0;");

importGroup.QueueFindReplace("gml_Object_obj_initializer2_Step_0", @"        if (global.is_console)
            global.screen_border_alpha = 1;", "global.screen_border_alpha = 1;");

importGroup.QueueAppend("gml_Object_obj_initializer2_Step_0", "global.game_won = scr_completed_chapter_any_slot(global.chapter);");

// scr_load
importGroup.QueueFindReplace("gml_GlobalScript_scr_load", @"    if (global.is_console)
        global.tempflag[95] = 1;", "global.tempflag[95] = 1;");

// obj_time

importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", @"if (global.is_console)
{
    if (!instance_exists(obj_gamecontroller))
        instance_create(0, 0, obj_gamecontroller);
    
    instance_create(0, 0, obj_border_controller);
}", @"if (global.is_console)
{
    if (!instance_exists(obj_gamecontroller))
        instance_create(0, 0, obj_gamecontroller);
}
    instance_create(0, 0, obj_border_controller);");


importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "if (display_width > (640 * _ww) && display_height > (480 * _ww))", "if (display_width > (640 * _ww) && display_height > (360 * _ww))");

importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");

importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", @"    if (global.is_console)
    {
        application_surface_enable(true);
        application_surface_draw_enable(false);
    }", @"application_surface_enable(true);
application_surface_draw_enable(false);");

importGroup.QueueFindReplace("gml_Object_obj_time_Create_0", "scr_enable_screen_border(global.is_console);", "scr_enable_screen_border(true);");

importGroup.QueueFindReplace("gml_Object_obj_time_Alarm_1", "window_set_size(640 * window_size_multiplier, 480 * window_size_multiplier);", "window_set_size(640 * window_size_multiplier, 360 * window_size_multiplier);");

importGroup.QueueFindReplace("gml_Object_obj_time_Step_0", "if (global.is_console)", "if (true)");

importGroup.QueueFindReplace("gml_Object_obj_time_Draw_64", "draw_sprite_ext(scr_84_get_sprite(\"spr_quitmessage\"), quit_timer / 7, 4, 4, 2, 2, 0, c_white, quit_timer / 15);", " draw_sprite_ext(scr_84_get_sprite(\"spr_quitmessage\"), quit_timer / 7, 40, 30, 2, 2, 0, c_white, quit_timer / 15);");

// obj_border_controller

importGroup.QueueFindReplace("gml_Object_obj_border_controller_Draw_77", @"var xx = floor((ww - (sw * global.window_scale)) / 2);
var yy = floor((wh - (sh * global.window_scale)) / 2);",
@"var border_w = 1920;
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
}");

importGroup.QueueFindReplace("gml_Object_obj_border_controller_Draw_77", "draw_surface_ext(application_surface, xx, yy, global.window_scale, global.window_scale, 0, c_white, 1);", "draw_surface_stretched(application_surface, xx, yy, ww - (2 * xx), wh - (2 * yy));");


// scr_draw_background_ps4

importGroup.QueueFindReplace("gml_GlobalScript_scr_draw_background_ps4", @"    if (os_type == os_ps4 || scr_is_switch_os() || os_type == os_ps5)
    {
        var scale = window_get_width() / 1920;
        draw_background_stretched(bg, xx * scale, yy * scale, background_get_width(bg) * scale, background_get_height(bg) * scale);
    }
    else
    {
        var scale = window_get_width() / 1920;
        draw_background_stretched(bg, xx * scale, yy * scale, background_get_width(bg) * scale, background_get_height(bg) * scale);
    }",
    @"var ww = window_get_width();
    var wh = window_get_height();
    var border_w = background_get_width(bg);
    var border_h = background_get_height(bg);
    var border_aspect = border_w / border_h;
    var window_aspect = ww / wh;
    var scale;
    
    if (window_aspect > border_aspect)
        scale = wh / border_h;
    else
        scale = ww / border_w;
    
    var draw_w = border_w * scale;
    var draw_h = border_h * scale;
    var draw_x = (ww - draw_w) / 2;
    var draw_y = (wh - draw_h) / 2;
    draw_background_stretched(bg, draw_x, draw_y, draw_w, draw_h);");

// obj_darkcontroller

importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Draw_0", "draw_sprite(spr_heart, 0, _heartXPos, yy + 160 + (global.submenucoord[30] * 35));", "draw_sprite(spr_heart, 0, _heartXPos, yy + 140 + (global.submenucoord[30] * 35));");

importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"        draw_text(_xPos, yy + 150, string_hash_to_newline(stringsetloc(""Master Volume"", ""obj_darkcontroller_slash_Draw_0_gml_86_0"")));
        draw_text(_selectXPos, yy + 150, string_hash_to_newline(audvol));
        draw_set_color(c_white);
        draw_text(_xPos, yy + 185, string_hash_to_newline(stringsetloc(""Controls"", ""obj_darkcontroller_slash_Draw_0_gml_91_0"")));
        draw_text(_xPos, yy + 220, string_hash_to_newline(stringsetloc(""Simplify VFX"", ""obj_darkcontroller_slash_Draw_0_gml_92_0"")));
        draw_text(_selectXPos, yy + 220, string_hash_to_newline(flashoff));",
        @"        draw_text(_xPos, yy + 130, string_hash_to_newline(stringsetloc(""Master Volume"", ""obj_darkcontroller_slash_Draw_0_gml_86_0"")));
        draw_text(_selectXPos, yy + 130, string_hash_to_newline(audvol));
        draw_set_color(c_white);
        draw_text(_xPos, yy + 165, string_hash_to_newline(stringsetloc(""Controls"", ""obj_darkcontroller_slash_Draw_0_gml_91_0"")));
        draw_text(_xPos, yy + 200, string_hash_to_newline(stringsetloc(""Simplify VFX"", ""obj_darkcontroller_slash_Draw_0_gml_92_0"")));
        draw_text(_selectXPos, yy + 200, string_hash_to_newline(flashoff));");

importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Draw_0", @"        if (global.is_console)
        {
            draw_text(_xPos, yy + 255, string_hash_to_newline(autorun_text));
            draw_text(_selectXPos, yy + 255, string_hash_to_newline(runoff));
            
            if (global.submenu == 36)
                draw_set_color(c_yellow);
            else if (global.disable_border)
                draw_set_color(c_gray);
            
            draw_text(_xPos, yy + 290, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
            draw_text(_selectXPos, yy + 290, border_options[selected_border]);
            draw_set_color(c_white);
            draw_text(_xPos, yy + 325, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 360, string_hash_to_newline(back_text));
        }
        else
        {
            draw_text(_xPos, yy + 255, string_hash_to_newline(stringsetloc(""Fullscreen"", ""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
            draw_text(xx + 430, yy + 255, string_hash_to_newline(fullscreenoff));
            draw_text(_xPos, yy + 290, string_hash_to_newline(autorun_text));
            draw_text(xx + 430, yy + 290, string_hash_to_newline(runoff));
            draw_text(_xPos, yy + 325, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
            draw_text(_xPos, yy + 360, string_hash_to_newline(back_text));
        }",
        @"        draw_text(_xPos, yy + 235, string_hash_to_newline(stringsetloc(""Fullscreen"", ""obj_darkcontroller_slash_Draw_0_gml_93_0"")));
        draw_text(_selectXPos, yy + 235, string_hash_to_newline(fullscreenoff));
        draw_text(_xPos, yy + 270, string_hash_to_newline(autorun_text));
        draw_text(_selectXPos, yy + 270, string_hash_to_newline(runoff));
        if (global.submenu == 36)
            draw_set_color(c_yellow);
        else if (global.disable_border)
            draw_set_color(c_gray);
        
        draw_text(_xPos, yy + 305, stringsetloc(""Border"", ""obj_darkcontroller_slash_Draw_0_gml_112_0""));
        draw_text(_selectXPos, yy + 305, border_options[selected_border]);
        draw_set_color(c_white);
        draw_text(_xPos, yy + 340, string_hash_to_newline(stringsetloc(""Return to Title"", ""obj_darkcontroller_slash_Draw_0_gml_95_0"")));
        draw_text(_xPos, yy + 375, string_hash_to_newline(back_text));");

importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.is_console && global.submenu == 36)", "if (global.submenu == 36)");

importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "if (global.submenucoord[30] > 6)", "if (global.submenucoord[30] > 7)");

importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", "global.submenucoord[30] = 6;", "global.submenucoord[30] = 7;");

importGroup.QueueFindReplace("gml_Object_obj_darkcontroller_Step_0", @"                if (global.is_console)
                {
                    if (global.submenucoord[30] == 3)
                    {
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }
                    
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.disable_border)
                        {
                            selectnoise = 0;
                        }
                        else
                        {
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }
                    }
                    
                    if (global.submenucoord[30] == 5)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 6)
                    {
                        m_quit = 1;
                        cancelnoise = 1;
                    }
                }
                else
                {
                    if (global.submenucoord[30] == 3)
                    {
                        with (obj_time)
                            fullscreen_toggle = 1;
                    }
                    
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }
                    
                    if (global.submenucoord[30] == 5)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 6)
                    {
                        m_quit = 1;
                        cancelnoise = 1;
                    }
                }",
                @"if (global.submenucoord[30] == 3)
                    {
                        with (obj_time)
                            fullscreen_toggle = 1;
                    }
                    
                    if (global.submenucoord[30] == 4)
                    {
                        if (global.flag[11] == 0)
                            global.flag[11] = 1;
                        else
                            global.flag[11] = 0;
                    }

                    if (global.submenucoord[30] == 5)
                    {
                        if (global.disable_border)
                        {
                            selectnoise = 0;
                        }
                        else
                        {
                            global.submenu = 36;
                            check_border = 1;
                            border_select = 0;
                        }
                    }
                    
                    if (global.submenucoord[30] == 6)
                        global.submenu = 34;
                    
                    if (global.submenucoord[30] == 7)
                    {
                        m_quit = 1;
                        cancelnoise = 1;
                    }");

// DEVICE_MENU

importGroup.QueueFindReplace("gml_Object_DEVICE_MENU_Step_0", @"if (!global.is_console)
                        {
                            ini_close();
                        }", @"if (!global.is_console)
                        {
                            global.screen_border_id = ini_read_string(""BORDER"", ""TYPE"", ""Dynamic"");
                            var _disable_border = global.screen_border_id == ""None"" || global.screen_border_id == ""なし"";
                            scr_enable_screen_border(!_disable_border);
                            ini_close();
                        }");

importGroup.QueueFindReplace("gml_Object_DEVICE_MENU_Alarm_0", "if (global.is_console)", "if (true)");

// scr_text

importGroup.QueueFindReplace("gml_GlobalScript_scr_text", "if (!paptalk && global.is_console)", "if (!paptalk)");

// obj_onion_event

importGroup.QueueFindReplace("gml_Object_obj_onion_event_Create_0", "if (global.is_console)", "if (true)");

importGroup.Import();

ScriptMessage("All done! :3");