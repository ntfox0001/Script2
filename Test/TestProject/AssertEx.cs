using NUnit.Framework;

namespace TestProject;

public class AssertEx
{
    public static Exception? Throws<T1>(Action action) where T1 : Exception 
    {
        try
        {
            action();
            Assert.Fail("Expected exception not thrown");
        }
        catch (T1 ex)
        {
            return ex;
        }

        return null;
    }
    
    public static Exception? Throws<T1, T2>(Action action) where T1 : Exception where T2 : Exception
    {
        try
        {
            action();
            Assert.Fail("Expected exception not thrown");
        }
        catch (T1 ex)
        {
            return ex;
        }
        catch (T2 ex)
        {
            return ex;
        }

        return null;
    }
    
    public static Exception? Throws<T1, T2, T3>(Action action) where T1 : Exception where T2 : Exception where T3 : Exception
    {
        try
        {
            action();
            Assert.Fail("Expected exception not thrown");
        }
        catch (T1 ex)
        {
            return ex;
        }
        catch (T2 ex)
        {
            return ex;
        }
        catch (T3 ex)
        {
            return ex;
        }
        
        return null;
    }
}