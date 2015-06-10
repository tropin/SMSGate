DECLARE @First XML, @Second XML

SET @First = '
<ArrayOfDestinationAddress xmlns="http://schemas.datacontract.org/2004/07/RoaminSMPP.Utility" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
		<DestinationAddress>
			<DestAddress>79225443636</DestAddress>
			<DestinationAddressTon>1</DestinationAddressTon>
			<IsDistributionList>false</IsDistributionList>
		</DestinationAddress>
		<DestinationAddress>
			<DestAddress>79225443637</DestAddress>
			<DestinationAddressTon>1</DestinationAddressTon>
			<IsDistributionList>false</IsDistributionList>
		</DestinationAddress>
</ArrayOfDestinationAddress>'
SET @Second =
	'<ArrayOfKeyValueOfanyTypeanyType xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
		<KeyValueOfanyTypeanyType>
			<Key i:type="a:unsignedShort" xmlns:a="http://www.w3.org/2001/XMLSchema">1281</Key>
			<Value i:type="a:base64Binary" xmlns:a="http://www.w3.org/2001/XMLSchema">MTIzNA==</Value>
		</KeyValueOfanyTypeanyType>
		<KeyValueOfanyTypeanyType>
			<Key i:type="a:unsignedShort" xmlns:a="http://www.w3.org/2001/XMLSchema">6</Key>
			<Value i:type="a:base64Binary" xmlns:a="http://www.w3.org/2001/XMLSchema">NDMyMQ==</Value>
		</KeyValueOfanyTypeanyType>
	</ArrayOfKeyValueOfanyTypeanyType>'
	
SELECT  Adressses.Item.value('./*[local-name()=''DestAddress''][1]','VARCHAR(21)') AS Destination,
	   Adressses.Item.value('./*[local-name()=''DestinationAddressTon''][1]','SMALLINT') AS Ton,
	   Adressses.Item.value('./*[local-name()=''DestinationAddressNpi''][1]','SMALLINT') AS Npi,
	   Adressses.Item.value('if (./*[local-name()=''IsDistributionList''][1] = ''true'') then 1 else 0','SMALLINT') AS IsDistributionList
FROM @First.nodes('//*[local-name()=''DestinationAddress'']') AS Adressses(Item);

SELECT TLV.Item.value('./*[local-name()=''Key''][1]','SMALLINT') AS Tag,
	   TLV.Item.value('./*[local-name()=''Value''][1]','VARCHAR(254)') AS Value
FROM @Second.nodes('//*[local-name()=''KeyValueOfanyTypeanyType'']') AS TLV(Item);

