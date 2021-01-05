# Starting the Program
1. Start the Service on the Server in WhatsChatSimple\WhatsChatSimpleV2\WhatsChat\ChatService\bin\Debug
2. Start the Client on each Client-Computer in WhatsChatSimple\WhatsChatSimpleV2\WhatsChat\WhatsChat\bin\Debug

# Production
If you have a dedicated Server you might want to change the Port, which you can do in WhatsChatSimple\WhatsChatSimpleV2\WhatsChat\ChatService\Program.cs

# Issues
If you get the System.ServiceModel.AddressAccessDeniedException try starting a cmd with Admin rights and run:

netsh http add urlacl url=http://+:2310/ user=mylocaluser

(insert the Servers Username instead of "mclocaluser")