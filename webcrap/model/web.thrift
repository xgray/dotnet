namespace csharp webcrap.web


#region Assembly System.Net.Primitives, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// <in-memory assembly>
#endregion

struct Cookie {
  1: string name,
  2: string value,
  3: i32 version,
  4: string comment,
  5: string commentUri,
  6: string domain,
  7: string port,
  8: string path,

  9: bool expired,
  10: i64 expires,
  11: bool httpOnly,
  12: bool secure,
  13: i64 timestamp
}

struct Cookies
{
  1: map<string, list<Cookie>> cookies
}

struct Simple {
  1: string value,
  2: i16 shortValue,
  3: i32 intValue,
  4: i64 longValue
}

struct OneOfEach {
  1: bool boolean_field,
  2: i8 a_bite,
  3: i16 integer16,
  4: i32 integer32,
  5: i64 integer64,
  6: double double_precision,
  7: string some_characters,
  8: binary base64,
  9: list<i8> byte_list,
  10: list<i16> i16_list,
  11: list<i64> i64_list
}

struct Complex {
  1: Simple simpleValue
  2: list<Simple> listValue
  3: set<Simple> setValue
  4: map<string,Simple> mapValue
  5: list<set<map<string,list<Simple>>>> listSetMap
  6: map<string, list<map<string,Simple>> > mapList
  7: list<list<string>> listOfList
  8: list<list<list<string>>> listOfListOfList
}

struct ComplexList {
  1: list<Complex> struct_list_field;
}

struct RecTree {
  1: list<RecTree> children
  2: i16 item
}

struct RecList {
  1: RecList & nextitem
  3: i16 item
}

struct CoRec {
  1:  CoRec2 & other
}

struct CoRec2 {
  1: CoRec other
}

struct VectorTest {
  1: list<RecList> lister;
}
