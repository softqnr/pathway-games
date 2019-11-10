using System.IO;

namespace PathwayGames.Extensions
{
    public static class FileStreamExtensions
    {
        public static long Seek(this FileStream fs, string searchString)
        {
            char[] search = searchString.ToCharArray();
            long result = -1;
            long position = 0;
            long stored = -1;
            long begin = fs.Position;
            int c;

            //read byte by byte
            while ((c = fs.ReadByte()) != -1)
            {
                //check if data in array matches
                if ((char)c == search[position])
                {
                    //if charater matches first character of 
                    //seek string, store it for later
                    if (stored == -1 && position > 0
                        && (char)c == search[0])
                    {
                        stored = fs.Position;
                    }

                    //check if we're done
                    if (position + 1 == search.Length)
                    {
                        //correct position for array lenth
                        result = fs.Position - search.Length;
                        //set position in stream
                        fs.Position = result;
                        break;
                    }

                    //advance position in the array
                    position++;
                }
                //no match, check if we have a stored position
                else if (stored > -1)
                {
                    //go to stored position + 1
                    fs.Position = stored + 1;
                    position = 1;
                    stored = -1; //reset stored position!
                }
                //no match, no stored position, reset array
                //position and continue reading
                else
                {
                    position = 0;
                }
            }

            //reset stream position if no match has been found
            if (result == -1)
            {
                fs.Position = begin;
            }

            return result;
        }
    }
}