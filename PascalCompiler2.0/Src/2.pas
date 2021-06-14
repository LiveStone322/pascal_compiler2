program test1;
type a = array [0 .. 9] of integer;
var b: a;
	c, d: boolean;
	e: integer;
begin
  writeln("Hello world!);
  c := not false and true or false;
  d := 1 > 0;

  if d then
    begin
	 writeln("d);
	 writeln("d");
	end;
  else writeln("not d");

  while d do
    begin
      d := false;
    end;
end.