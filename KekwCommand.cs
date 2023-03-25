using System.CommandLine;
using Spectre.Console;

public class KekwCommand : Command
{
  public KekwCommand() : base("kekw", "get a list of all available os images")
  {
    this.IsHidden = true;
    
    var kekw = """
      [s[?25l[48;2;156;93;81m[38;2;132;79;63m▄[48;2;159;92;76m[38;2;93;44;30m▄[48;2;183;108;83m[38;2;89;40;26m▄[48;2;199;118;95m[38;2;97;45;31m▄[48;2;201;116;92m[38;2;107;45;24m▄[48;2;218;129;99m[38;2;154;84;57m▄[48;2;245;142;112m[38;2;205;113;89m▄[48;2;249;145;116m[38;2;231;130;108m▄[48;2;237;134;105m[38;2;223;123;104m▄[48;2;229;130;99m[38;2;198;104;84m▄[48;2;226;126;96m[38;2;175;96;80m▄[48;2;236;146;118m[38;2;148;74;58m▄[48;2;249;164;137m[38;2;146;75;61m▄[48;2;237;144;115m[38;2;157;86;72m▄[48;2;206;118;87m[38;2;145;75;57m▄[48;2;193;108;78m[38;2;142;72;54m▄[48;2;184;99;69m[38;2;153;80;58m▄[48;2;184;99;68m[38;2;164;88;65m▄[48;2;182;98;67m[38;2;172;92;64m▄[48;2;185;101;70m[38;2;178;95;65m▄[m
      [48;2;108;54;44m[38;2;122;69;51m▄[48;2;106;55;41m[38;2;146;87;75m▄[48;2;110;57;44m[38;2;166;100;90m▄[48;2;107;53;42m[38;2;178;104;96m▄[48;2;111;50;38m[38;2;189;107;98m▄[48;2;126;54;37m[38;2;169;87;73m▄[48;2;177;97;77m[38;2;144;60;41m▄[48;2;193;98;78m[38;2;173;81;57m▄[48;2;211;114;95m[38;2;210;115;90m▄[48;2;176;90;71m[38;2;195;98;80m▄[48;2;137;67;56m[38;2;159;72;57m▄[48;2;105;48;39m[38;2;144;73;57m▄[48;2;87;30;26m[38;2;168;95;82m▄[48;2;90;32;29m[38;2;179;108;95m▄[48;2;115;58;53m[38;2;191;119;110m▄[48;2;137;80;69m[38;2;193;125;111m▄[48;2;160;94;77m[38;2;184;110;96m▄[48;2;166;92;64m[38;2;175;99;74m▄[48;2;165;90;61m[38;2;167;91;66m▄[48;2;169;90;59m[38;2;168;89;61m▄[m
      [48;2;99;47;33m[38;2;157;90;71m▄[48;2;76;21;17m[38;2;136;69;53m▄[48;2;88;30;26m[38;2;150;71;55m▄[48;2;131;57;47m[38;2;173;92;75m▄[48;2;163;85;74m[38;2;184;96;79m▄[48;2;175;92;74m[38;2;185;98;73m▄[48;2;184;97;75m[38;2;188;96;70m▄[48;2;190;95;71m[38;2;200;100;73m▄[48;2;235;136;111m[38;2;222;118;90m▄[48;2;213;111;86m[38;2;216;112;83m▄[48;2;198;99;72m[38;2;208;105;75m▄[48;2;177;84;61m[38;2;203;105;74m▄[48;2;167;86;70m[38;2;188;97;71m▄[48;2;133;57;42m[38;2;165;85;61m▄[48;2;99;29;21m[38;2;190;113;90m▄[48;2;106;42;31m[38;2;202;129;103m▄[48;2;113;47;30m[38;2;196;121;98m▄[48;2;123;51;30m[38;2;183;104;81m▄[48;2;149;73;50m[38;2;170;86;61m▄[48;2;165;85;62m[38;2;181;96;71m▄[m
      [48;2;182;93;71m[38;2;185;92;67m▄[48;2;176;84;63m[38;2;193;92;69m▄[48;2;176;77;51m[38;2;215;107;80m▄[48;2;199;100;72m[38;2;218;111;85m▄[48;2;196;101;73m[38;2;197;102;74m▄[48;2;185;95;66m[38;2;180;89;60m▄[48;2;184;93;64m[38;2;178;88;57m▄[48;2;197;97;70m[38;2;193;96;68m▄[48;2;215;109;83m[38;2;203;99;74m▄[48;2;214;109;82m[38;2;198;94;69m▄[48;2;210;108;78m[38;2;193;92;65m▄[48;2;206;105;75m[38;2;197;97;69m▄[48;2;208;110;80m[38;2;222;124;95m▄[48;2;213;116;87m[38;2;236;136;105m▄[48;2;191;94;65m[38;2;219;119;89m▄[48;2;190;93;63m[38;2;226;123;93m▄[48;2;202;97;70m[38;2;238;137;107m▄[48;2;209;105;78m[38;2;246;147;116m▄[48;2;203;104;73m[38;2;215;114;84m▄[48;2;206;108;77m[38;2;206;108;78m▄[m
      [48;2;189;94;67m[38;2;188;95;67m▄[48;2;202;97;73m[38;2;195;100;74m▄[48;2;215;107;79m[38;2;195;100;78m▄[48;2;209;103;80m[38;2;178;86;64m▄[48;2;186;93;67m[38;2;165;86;67m▄[48;2;176;85;60m[38;2;156;77;63m▄[48;2;187;96;73m[38;2;163;83;71m▄[48;2;214;121;97m[38;2;175;94;80m▄[48;2;205;110;85m[38;2;172;88;75m▄[48;2;177;79;55m[38;2;170;85;72m▄[48;2;180;81;57m[38;2;187;102;89m▄[48;2;189;90;66m[38;2;181;96;82m▄[48;2;184;90;63m[38;2;164;79;63m▄[48;2;219;129;98m[38;2;196;109;89m▄[48;2;248;158;126m[38;2;204;112;87m▄[48;2;248;158;126m[38;2;236;139;112m▄[48;2;251;166;133m[38;2;231;138;107m▄[48;2;241;146;114m[38;2;215;118;86m▄[48;2;219;117;88m[38;2;206;107;76m▄[48;2;205;109;78m[38;2;196;103;70m▄[m
      [48;2;157;79;54m[38;2;129;66;46m▄[48;2;168;85;67m[38;2;139;67;50m▄[48;2;173;90;76m[38;2;142;73;57m▄[48;2;144;71;56m[38;2;106;61;50m▄[48;2;125;71;59m[38;2;55;25;22m▄[48;2;76;24;18m[38;2;49;19;16m▄[48;2;92;40;35m[38;2;66;30;28m▄[48;2;108;57;50m[38;2;78;38;33m▄[48;2;100;46;40m[38;2;97;54;48m▄[48;2;87;31;26m[38;2;76;33;27m▄[48;2;92;36;31m[38;2;85;46;39m▄[48;2;108;51;45m[38;2;77;37;30m▄[48;2;126;70;60m[38;2;92;51;45m▄[48;2;145;87;73m[38;2;90;50;45m▄[48;2;161;89;74m[38;2;111;61;54m▄[48;2;174;93;70m[38;2;138;74;60m▄[48;2;195;104;80m[38;2;159;83;67m▄[48;2;195;98;76m[38;2;176;88;70m▄[48;2;187;91;67m[38;2;171;85;62m▄[48;2;182;91;67m[38;2;163;85;59m▄[m
      [48;2;127;64;47m[38;2;127;65;48m▄[48;2;132;68;56m[38;2;131;68;56m▄[48;2;126;73;56m[38;2;123;68;56m▄[48;2;70;36;25m[38;2;52;14;7m▄[48;2;45;24;18m[38;2;56;25;23m▄[48;2;50;27;22m[38;2;60;28;24m▄[48;2;58;27;20m[38;2;54;16;12m▄[48;2;71;30;23m[38;2;87;39;36m▄[48;2;102;56;51m[38;2;133;85;81m▄[48;2;64;21;15m[38;2;96;48;45m▄[48;2;81;43;35m[38;2;91;48;43m▄[48;2;65;30;22m[38;2;77;40;33m▄[48;2;78;44;35m[38;2;85;51;41m▄[48;2;75;40;31m[38;2;85;51;40m▄[48;2;78;42;31m[38;2;73;39;31m▄[48;2;104;54;40m[38;2;69;32;22m▄[48;2;131;73;54m[38;2;91;45;32m▄[48;2;153;80;63m[38;2;138;75;63m▄[48;2;158;86;62m[38;2;154;88;67m▄[48;2;157;87;62m[38;2;155;90;65m▄[m
      [48;2;121;63;50m[38;2;116;59;48m▄[48;2;126;68;54m[38;2;121;62;49m▄[48;2;135;80;68m[38;2;149;87;73m▄[48;2;74;29;17m[38;2;152;89;76m▄[48;2;87;42;31m[38;2;135;71;61m▄[48;2;74;25;19m[38;2;113;54;47m▄[48;2;128;80;80m[38;2;116;69;67m▄[48;2;68;26;28m[38;2;70;28;28m▄[48;2;44;5;5m[38;2;69;27;26m▄[48;2;53;14;15m[38;2;56;11;12m▄[48;2;52;14;14m[38;2;55;10;9m▄[48;2;49;14;13m[38;2;66;21;21m▄[48;2;54;19;17m[38;2;81;36;36m▄[48;2;57;22;20m[38;2;88;42;42m▄[48;2;64;30;28m[38;2;82;38;37m▄[48;2;79;40;33m[38;2;73;23;20m▄[48;2;77;29;22m[38;2;131;72;68m▄[48;2;118;54;43m[38;2;142;78;66m▄[48;2;153;89;72m[38;2;155;92;74m▄[48;2;153;90;72m[38;2;150;89;67m▄[m
      [48;2;85;47;41m[38;2;74;42;34m▄[48;2;125;63;51m[38;2;121;64;55m▄[48;2;142;69;51m[38;2;152;81;62m▄[48;2;168;95;76m[38;2;156;83;64m▄[48;2;151;82;68m[38;2;157;84;66m▄[48;2;166;99;87m[38;2;164;91;75m▄[48;2;125;60;50m[38;2;143;69;58m▄[48;2;168;102;95m[38;2;197;123;114m▄[48;2;203;135;128m[38;2;207;133;124m▄[48;2;201;132;126m[38;2;200;126;117m▄[48;2;203;132;127m[38;2;196;122;113m▄[48;2;209;135;131m[38;2;189;114;105m▄[48;2;203;130;125m[38;2;186;107;99m▄[48;2;180;107;102m[38;2;157;79;66m▄[48;2;146;78;71m[38;2;167;90;77m▄[48;2;147;78;72m[38;2;187;110;92m▄[48;2;156;89;78m[38;2;161;84;67m▄[48;2;149;83;66m[38;2;158;85;66m▄[48;2;158;92;71m[38;2;155;88;66m▄[48;2;146;84;61m[38;2;146;81;59m▄[m
      [48;2;73;35;33m[38;2;71;36;33m▄[48;2;122;65;56m[38;2;117;68;60m▄[48;2;162;91;72m[38;2;149;79;60m▄[48;2;159;86;67m[38;2;167;95;76m▄[48;2;163;90;71m[38;2;159;87;70m▄[48;2;164;89;70m[38;2;155;82;66m▄[48;2;143;66;52m[38;2;157;84;69m▄[48;2;164;88;78m[38;2;163;90;76m▄[48;2;170;96;87m[38;2;169;96;82m▄[48;2;173;99;90m[38;2;168;94;81m▄[48;2;171;97;89m[38;2;175;98;86m▄[48;2;177;102;94m[38;2;208;130;118m▄[48;2;187;108;95m[38;2;191;110;94m▄[48;2;164;81;66m[38;2;169;87;70m▄[48;2;179;97;81m[38;2;184;103;82m▄[48;2;184;108;84m[38;2;183;107;83m▄[48;2;169;93;69m[38;2;169;96;72m▄[48;2;164;91;66m[38;2;165;93;69m▄[48;2;154;87;65m[38;2;152;86;64m▄[48;2;144;78;56m[38;2;152;87;65m▄[m
      [?25h
      """;

    this.SetHandler(() =>
    {
      Console.WriteLine(kekw);
    });
  }
}