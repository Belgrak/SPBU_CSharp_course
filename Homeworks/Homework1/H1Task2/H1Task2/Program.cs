using H1Task2;

const string example = "ABACABA";
Console.WriteLine("Before: " + example);
var transformed = BurrowsWheelerTransformation.Transform(example);
Console.WriteLine("After BWT: " + transformed.Item1 + ", " + transformed.Item2);
var reversed = BurrowsWheelerTransformation.ReverseTransform(transformed.Item1, transformed.Item2);
Console.WriteLine("After BWT reversed: " + reversed);