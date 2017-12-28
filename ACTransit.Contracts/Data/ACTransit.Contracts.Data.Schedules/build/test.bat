
set contract.Mobile.pattern="^<member.{0,4}name="".:ACTransit^\.CusRel.*?"^>.*?^<summary^>.*?^</summary^>.*^?^<^/member^>^\s++"

regex C:\Projects\ACTransit.Projects\trunk\ACTransit.Contracts\ACTransit.Contracts.DataContracts\bin\Debug\ACTransit.Contracts.DataContracts.Mobile.XML %contract.Mobile.pattern%