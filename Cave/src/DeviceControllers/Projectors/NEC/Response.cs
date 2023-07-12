using System;
using System.Reflection;

namespace Cave.DeviceControllers.Projectors.NEC
{
    public class Response
    {
        public byte[] Data { get; protected set; }
        public string? Name { get; init; }

        /* Wildcard byte */
        private static byte wild = (byte)'*';


        /* Success responses */
        public static Response PowerOnSuccess = new( new byte[] { 0x22, 0x00, wild, wild, 0x00, wild }, nameof(PowerOnSuccess) );
        public static Response PowerOffSuccess = new( new byte[] { 0x22, 0x01, wild, wild, 0x00, wild }, nameof(PowerOffSuccess) );
        public static Response SelectInputSuccess = new( new byte[] { 0x22, 0x03, wild, wild, 0x01, wild, wild }, nameof(SelectInputSuccess) );
        public static Response GetStatusSuccess = new( new byte[] { 0x20, 0xbf, wild, wild, 0x10, 0x02, 
        /*  Power       Content displayed       Input selected (tuple)      Video signal type */
            wild,       wild,                   wild, wild,                 wild,
        /*  Video mute  Sound mute      Onscreen mute   Freeze status   System reserved */
            wild,       wild,           wild,           wild,           wild, wild, wild, wild, wild, wild,
        /*  Checksum */    
            wild }, nameof(GetStatusSuccess) );

        public static Response GetLampInfoSuccess = new( new byte[] { 0x23, 0x96, wild, wild, 0x06,
        /*  Lamp#       What was requested      32-bit int data             Checksum */
            wild,       wild,                   wild, wild, wild, wild,     wild }, nameof(GetLampInfoSuccess) );

        public static Response GetErrorsSuccess = new( new byte[] { 0x20, 0x88, wild, wild, 0x0c,
        /*  Data 1 - 4 error flags */
            wild, wild, wild, wild, 
        /*  Data 5 - 8 reserved for system */
            wild, wild, wild, wild,
        /*  Data 9 extended error status flags */
            wild,
        /*  Data 10 - 12 reserved for system */
            wild, wild, wild,
        /*  Checksum */
            wild }, nameof(GetErrorsSuccess) );

        public static Response GetModelInfoSuccess = new( new byte[] { 0x20, 0x85, wild, wild, 0x20, 
        /*  32 bytes of data */
            wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild,
            wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild,
        /*  Checksum */
            wild }, nameof(GetModelInfoSuccess) );

        public static Response GetSerialInfoSuccess = new( new byte[] { 0x20, 0xbf, wild, wild, 0x12, 0x01, 0x06,
        /*  16 bytes of data */
            wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild, wild,
        /*  Checksum */
            wild }, nameof(GetSerialInfoSuccess) );


        /* Failure responses */
        public static Response PowerOnFailure = new( new byte[] { 0xa2, 0x00, wild, wild, 0x02, wild, wild, wild }, nameof(PowerOnFailure) );
        public static Response PowerOffFailure = new( new byte[] { 0xa2, 0x01, wild, wild, 0x02, wild, wild, wild }, nameof(PowerOffFailure) );
        public static Response SelectInputFailure = new( new byte[] { 0xa2, 0x03, wild, wild, 0x02, wild, wild, wild }, nameof(SelectInputFailure) );
        public static Response GetLampInfoFailure = new( new byte[] { 0xa3, 0x96, wild, wild, 0x02, wild, wild, wild }, nameof(GetLampInfoFailure) );
        public static Response GetErrorsFailure = new( new byte[] { 0xa0, 0x88, wild, wild, 0x02, wild, wild, wild }, nameof(GetErrorsFailure) );

        public Response( byte[] data, string? name=null ) { Data = data; Name = name; }

        /* Copy constructor for testing */
        public Response( Response other )
        {
            Data = new byte[other.Data.Length];
            Array.Copy(other.Data, Data, Data.Length);
            Name = other.Name;
        }

        public bool IndicatesSuccess
        {
            get => ( this.Data[0] >> 4 == 0x02 );
        }

        public bool IndicatesFailure
        {
            get => ( this.Data[0] >> 4 == 0x0a );
        }

        /**
        * Checks a Response against another to see if they partially match.
        * Arrays must at least match in length.  Either array may contain '*' (0x2a)
        * for any byte positions, indicating that byte is a wildcard that does not
        * need to match.  If no non-wildcard byte mismatches are found, the two
        * Responses match.
        */
        public bool Matches( Response other )
        {
            return this.Matches(other.Data);
        }

        public bool Matches( byte[] other )
        {
            if( this.Data.Length != other.Length )
                return false;
            else
            {
                for( int idx = 0; idx < this.Data.Length; ++idx )
                {
                    if(
                        this.Data[idx] != other[idx] &&
                        this.Data[idx] != wild &&
                        other[idx] != wild
                    )
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static IEnumerable<Response> GetAll() =>
            typeof(Response).GetFields( BindingFlags.Public |
                                        BindingFlags.Static |
                                        BindingFlags.DeclaredOnly )
                            .Select(field => field.GetValue(null))
                            .Cast<Response>();

        public static Response? FromData( byte[] data )
        {
            var match = GetAll().FirstOrDefault( member => member.Matches(data) );
            return match;
        }

        public override string ToString()
        {
            string data = "";
            for( int idx = 0; idx < Data.Length; ++idx )
            {
                data += string.Format("0x{0:x2}", Data[idx]);
                if( idx < Data.Length - 1 )
                    data += " ";
            }
            return $"Response: {(Name ?? "[Unknown response]")} [{data}]";
        }
    }
}