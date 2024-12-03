using System.Runtime.CompilerServices;

// Tests.Runtime 어셈블리가 Assembly-CSharp의 internal 멤버에 접근할 수 있도록 허용
[assembly: InternalsVisibleTo("Tests")]
[assembly: InternalsVisibleTo("Tests.Runtime")]