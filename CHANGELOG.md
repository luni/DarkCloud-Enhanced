# Changelog

This changelog lists changes by date with links to the corresponding commits.

For the original v1.00 feature list from the release PDF, see [Original v1.00 Release Notes](#original-v100-release-notes).

## 2020-08-10

- Dayuppy added a comment for testing how to commit. ([`c48d7c5`](https://github.com/luni/DarkCloud-Enhanced/commit/c48d7c568c2317d7d0f2b46ecba1d48cf0f0116d))

## 2020-08-11

- Added functions required for obtaining processID and Reading and Writing Process memory. ([`1dacb99`](https://github.com/luni/DarkCloud-Enhanced/commit/1dacb993170e02deb27691a820ff54d6e9b99114))
- Cleaned up the code a bit. Added function for reading int from memory address. ([`0f33a01`](https://github.com/luni/DarkCloud-Enhanced/commit/0f33a01b967bd8156921c70b4ce5b74b3171319f))
- Added ReadShort and WriteShort functions. Implemented examples of Player and Dungeon class. ([`79194f8`](https://github.com/luni/DarkCloud-Enhanced/commit/79194f8bee1fe7e029b0756ef3a8a43870524c34))
- Seperated functions into seperate CS files. Implemented Read and WriteFloat, Player and Dungeon Class simplified. ([`b5920bf`](https://github.com/luni/DarkCloud-Enhanced/commit/b5920bf2555302ef55ac90d593d5cb38a2b00f01))
- Changed function accessibilites for better readability. Read and WriteFloat are not functioning correctly yet. ([`73003da`](https://github.com/luni/DarkCloud-Enhanced/commit/73003dabab9f52c8004431a974f030336330bb25))
- Added Toan and Xiao sub-classes. ([`f4d8498`](https://github.com/luni/DarkCloud-Enhanced/commit/f4d84983373c86185e7ec09ddaee191776743ae6))

## 2020-08-12

- Created an address class that will store most addresses we use ([`b6b5994`](https://github.com/luni/DarkCloud-Enhanced/commit/b6b59948556db9c7f21d5b271d65bb0a18392d02))
- Added more addresses ([`120c759`](https://github.com/luni/DarkCloud-Enhanced/commit/120c7599f65a77edb42bd29c2ea4aa0b726cfdba))
- Removed multiple duplicate functions and instead opted for less lines of code with slightly more typing. ([`314794e`](https://github.com/luni/DarkCloud-Enhanced/commit/314794e78249141a843ee3cfeaa9fb87c4e07dcf))
- Greatly simplified Read and Write functions by making better use of BitConverter. ([`2c36a86`](https://github.com/luni/DarkCloud-Enhanced/commit/2c36a86ad188c266a07ddcf56f78e96ff8b88e09))
- Added more addresses for Toan. ([`7f20b7a`](https://github.com/luni/DarkCloud-Enhanced/commit/7f20b7a636f687928374ab95c4515547aa19619c))
- Cleaned up the code. Provided better examples in main. ([`03d95cd`](https://github.com/luni/DarkCloud-Enhanced/commit/03d95cd3c1c75e5c622d02e6a5fe754cc161b6fd))
- Added MenuShortcuts class ([`c8f7c03`](https://github.com/luni/DarkCloud-Enhanced/commit/c8f7c037315c765bfc81048128d59421305925cc))
- Added buttons in the form for individual .cs testing purposes + Customchests class. ([`7a0de17`](https://github.com/luni/DarkCloud-Enhanced/commit/7a0de1729c63ad3d6c98d3c2802f684182deeaf5))

## 2020-08-13

- Cleaned up main. Fixed ReadByte. Created Dayuppy.cs. ([`aee5dc6`](https://github.com/luni/DarkCloud-Enhanced/commit/aee5dc6e48c28a936b16afad9a82ac76ed8a3459))
- Clarified button inputs and cleaned up some unnecessary addresses. ([`25279cf`](https://github.com/luni/DarkCloud-Enhanced/commit/25279cf9598e5d29f35b38c038a3932c46d8b91a))
- Added Custom Chests functionality ([`2bc0236`](https://github.com/luni/DarkCloud-Enhanced/commit/2bc0236a29d0a348ae74ded709c5f127ee65cf6f))
- Added soft resetting with L1+R1+Select+Start. ([`74ca582`](https://github.com/luni/DarkCloud-Enhanced/commit/74ca582a87347910f028c073695c72ac8696790f))

## 2020-08-14

- Added addresses. ([`104b3a7`](https://github.com/luni/DarkCloud-Enhanced/commit/104b3a7649a6d5d43ded3e1c46787eba4161d3b9))

## 2020-08-15

- Added working example of cheat codes. ([`0a2d4e4`](https://github.com/luni/DarkCloud-Enhanced/commit/0a2d4e4df836a765ca2dc1cb6dc741aeb5537895))
- Added Read and Write ByteArray functions. ([`c66cd3a`](https://github.com/luni/DarkCloud-Enhanced/commit/c66cd3a679c7c7368d20fb32c54e79f7b739833a))

## 2020-08-17

- Added God Mode with L1+L2+R1+R2+DpadUp. + Custom Message Example ([`83cdfed`](https://github.com/luni/DarkCloud-Enhanced/commit/83cdfed54ff2921d2a80f7a61183799805d207df))
- Fixed Include for FileIO. ([`4637d71`](https://github.com/luni/DarkCloud-Enhanced/commit/4637d71b2cb1a4875430b74749202bf36adf3118))
- Fixed issue if texture files does not exist. ([`394b7ef`](https://github.com/luni/DarkCloud-Enhanced/commit/394b7ef01bb27970a588468c528d82dd8dbcfa7b))
- Fixed ReadByte (again). Added example for changing Weapon Atrributes. ([`30c1e41`](https://github.com/luni/DarkCloud-Enhanced/commit/30c1e419a650db951443e3dbf237222cb6f61e0c))

## 2020-08-18

- Added element quickswapping for all characters and weapon slots. ([`cb57dbf`](https://github.com/luni/DarkCloud-Enhanced/commit/cb57dbf42b8ec1a5bbaf3790e5fdf7ff4014cf74))
- Added Dungeon Clear message. Added checks for element swapping. ([`3a7c8d1`](https://github.com/luni/DarkCloud-Enhanced/commit/3a7c8d1f6aa094302450c1d85c9463d63b80d3e3))

## 2020-08-19

- Added check if file exists for test.tm2. ([`bf98d58`](https://github.com/luni/DarkCloud-Enhanced/commit/bf98d58cfabaf1bf127bc437774204a09d14cf08))

## 2020-08-22

- Fixed multiple Memory Functions. Added example of inserting function call. ([`a8ac243`](https://github.com/luni/DarkCloud-Enhanced/commit/a8ac24322473883388f35976906d163036f4fdd3))

## 2020-08-23

- Added test chest randomizer function for DBC Level 1. ([`ed27f93`](https://github.com/luni/DarkCloud-Enhanced/commit/ed27f93c9e3bf20f14c3ab7dc4b4c67ae6d0719a))
- Added all dungeons and floors to Day's chest randomizer variant. ([`e9eb533`](https://github.com/luni/DarkCloud-Enhanced/commit/e9eb533f818089b6509c95a9e60c1971927cdc90))
- Called chest randomizer directly rather than running seperate thread as some of the function is not thread-safe. ([`b56bd3a`](https://github.com/luni/DarkCloud-Enhanced/commit/b56bd3a1abf015ce1636e42bc5f2aeaf8d397a8f))
- Add console output for weapon spawns. ([`d7b2cbd`](https://github.com/luni/DarkCloud-Enhanced/commit/d7b2cbd18bde722f506374537a7f7e97f318cbaa))

## 2020-08-25

- Added file patcher and suspend program feature. Cleaned up the code. ([`1044412`](https://github.com/luni/DarkCloud-Enhanced/commit/104441273519ab918a352abb943523092cb28252))

## 2020-08-26

- Fixed address offsets. Improved patcher. More memory search functions. ([`ac086d9`](https://github.com/luni/DarkCloud-Enhanced/commit/ac086d9e7f5f80333abea2a2a742bf6fe73a40f0))
- Fixed address offsets. ([`4fe3161`](https://github.com/luni/DarkCloud-Enhanced/commit/4fe31618623172ff0fe9985e40fb233b3562d912))

## 2020-08-28

- Added shop items, enemy drop randomizer, item addresses. ([`d851188`](https://github.com/luni/DarkCloud-Enhanced/commit/d8511886202dbced5e9a9ec60a4e1fcbe87759fa))

## 2020-08-30

- Added very buggy and basic cheat functions. ([`214216a`](https://github.com/luni/DarkCloud-Enhanced/commit/214216aebd53f39e06f64f2311d75903cce7e42d))
- Finalized some testing functions. ([`eccfe62`](https://github.com/luni/DarkCloud-Enhanced/commit/eccfe62be3f50e9aa221f7ecc437772f1099f745))

## 2020-09-23

- Started working on character switching in towns ([`ce2f7b0`](https://github.com/luni/DarkCloud-Enhanced/commit/ce2f7b0222f7c5fb524c9ea1480a7ca13a18a894))
- Created the Weapons.cs file ([`8f62b02`](https://github.com/luni/DarkCloud-Enhanced/commit/8f62b02200f7824624d96c7e346d5e77494c4c2c))
- Commit for merge ([`329f8ba`](https://github.com/luni/DarkCloud-Enhanced/commit/329f8ba815bf2cc6427962d12234e0d773a7bc83))

## 2020-10-04

- Added weapon changes (incomplete) ([`8563fc5`](https://github.com/luni/DarkCloud-Enhanced/commit/8563fc50e3c58cbc13786b803a0146f9c9bccfd7))
- Toan + Xiao weapon stats done ([`56cb2e9`](https://github.com/luni/DarkCloud-Enhanced/commit/56cb2e9549828455c89cae233a82bba5ce7e3395))

## 2020-10-08

- Goro weapons done + Ruby weapons almost finished ([`8555b67`](https://github.com/luni/DarkCloud-Enhanced/commit/8555b678d56da475a638ece4babe8de6161a81b9))
- Ruby weapon stat changes done ([`ba87f8b`](https://github.com/luni/DarkCloud-Enhanced/commit/ba87f8be84aea8681a6afe3e9cd773f9a964e301))

## 2020-10-10

- Ungaga and Osmond weapon chances complete ([`d0c5c9c`](https://github.com/luni/DarkCloud-Enhanced/commit/d0c5c9cb8b984c0de7fa79782995c7fe595fd162))

## 2020-10-12

- Ungaga's Babel spear special effect done ([`c5e19d1`](https://github.com/luni/DarkCloud-Enhanced/commit/c5e19d1187e3d71fc002130c068dae772042050f))

## 2020-10-15

- Dragon's Y custom effect ([`c77e845`](https://github.com/luni/DarkCloud-Enhanced/commit/c77e845f3c6b28a963dd4dfc8a1387a5fe28e7de))
- Updated Towncharacter class, work still in progress ([`17418fb`](https://github.com/luni/DarkCloud-Enhanced/commit/17418fb215398612df062d64cf303754385e7792))

## 2020-10-20

- Addresses update ([`b6131e5`](https://github.com/luni/DarkCloud-Enhanced/commit/b6131e5896ae8cf566f5d008fd2cc4de5701ef8a))

## 2020-10-25

- Ungaga Hercules Wrath finished + Code cleanup ([`5caf3c5`](https://github.com/luni/DarkCloud-Enhanced/commit/5caf3c580f9dba105266d412c2c5ecab9dcb9280))

## 2020-10-27

- Mostly finished functionality for allies in town, need to add dialogues next ([`b10ce7c`](https://github.com/luni/DarkCloud-Enhanced/commit/b10ce7c6b6f5ef6a56af533647306d4ad4d67b41))

## 2020-10-30

- Added ReusableFunctions.cs + Supernova update (Unfinished) ([`3beb117`](https://github.com/luni/DarkCloud-Enhanced/commit/3beb117b28226b1c3812132890e93585d278f371))

## 2020-11-25

- Added dialogue class and updates on Towncharacter ([`8684c84`](https://github.com/luni/DarkCloud-Enhanced/commit/8684c844ef7dc5e6f8a89d938a1f8c584ee56401))
- Added Ruby Custom Effect - Mobius Ring, Added RubyOrbs.cs, Added new resizing arguments on Dayuppy's DungeonMessageDisplay method, Varius other additions to Addresses.cs and Enemies.cs; ([`a4c6e69`](https://github.com/luni/DarkCloud-Enhanced/commit/a4c6e69f3f11976d0559b435c168fa89d5916e13))
- Added the "0" (zero) character to dayuppy's message function; Improved Ruby's custom effect - Mobius Ring; ([`1196ebf`](https://github.com/luni/DarkCloud-Enhanced/commit/1196ebf2414e8edb8f8fed332765e6a1102d9fbf))
- Finished Ruby's custom effect - Mobius Ring ([`cdd1da3`](https://github.com/luni/DarkCloud-Enhanced/commit/cdd1da38b90a4a689ecabe317d2152e13077f9f4))

## 2020-12-05

- Updated Dialogues to have placeholder dialogues for other characters ([`879e2b2`](https://github.com/luni/DarkCloud-Enhanced/commit/879e2b2f2df3d700d405823912a21177e23ab23e))
- Forgot to apply some dialogue lines ([`0c1794b`](https://github.com/luni/DarkCloud-Enhanced/commit/0c1794ba2d015af041df2b2846a6c571aed7a089))

## 2021-01-04

- Finished Norune dialogues + some test changes in other classes ([`281fadc`](https://github.com/luni/DarkCloud-Enhanced/commit/281fadc64e8a2d7f3bc9c47ed59f907372b522eb))

## 2021-01-10

- Reworked Threads and improved performance ([`ad5a0d8`](https://github.com/luni/DarkCloud-Enhanced/commit/ad5a0d89b05d50f99764bb489322a851c621ba34))
- Fixed bug with NPC dialogue changing when going next to a NPC ([`220cafc`](https://github.com/luni/DarkCloud-Enhanced/commit/220cafcb3ce42b6960a9f617c6a0ecee10b4efaf))

## 2021-01-20

- Extended TownCharacter and Dialogues to work in matataki (still need to add the dialogues itself) ([`740f389`](https://github.com/luni/DarkCloud-Enhanced/commit/740f389ac3078aa52f44f76b7d166fb970806310))

## 2021-01-28

- Tested TASThread (which didnt work out) ([`2412953`](https://github.com/luni/DarkCloud-Enhanced/commit/24129531ba299fe4f17639ffaa4c7951b7ee9ff7))

## 2021-02-07

- Added mini boss thread ([`2e34f88`](https://github.com/luni/DarkCloud-Enhanced/commit/2e34f88cf254943a54e80da7258dd3ce1fe976da))
- Miniboss ([`681d666`](https://github.com/luni/DarkCloud-Enhanced/commit/681d666d1e56b69f791a977ba1902baa5017127a))
- Fixed the misc files ([`bb23da1`](https://github.com/luni/DarkCloud-Enhanced/commit/bb23da1c858e6bce3bb28c90a7d0f5d74a2e4c83))

## 2021-02-08

- Added Tall Hammer custom effect ([`2619439`](https://github.com/luni/DarkCloud-Enhanced/commit/2619439b59b44527fc5c563001273a3fb3f164c1))

## 2021-02-09

- Added comments for the Tall Hammer and Hercules Wrath functions ([`c4e353f`](https://github.com/luni/DarkCloud-Enhanced/commit/c4e353f5619324a9940809da23307def7d335039))

## 2021-02-16

- Added rest of matataki village dialogues ([`713233f`](https://github.com/luni/DarkCloud-Enhanced/commit/713233f4b27b2b5903034b45819bdfe83da204d6))

## 2021-02-28

- Combined customchests and minibossthread into Dungeon.cs, added all original loot tables to customchests, added Chronicle 2 custom effect, added some more buttons to activate separate threads, and some other minor changes. ([`e8df435`](https://github.com/luni/DarkCloud-Enhanced/commit/e8df435056f5210adf532faa92d06492471c80ea))

## 2021-03-21

- Added dialogue possibility for Queens ([`9828b5a`](https://github.com/luni/DarkCloud-Enhanced/commit/9828b5a086155f42e4dfaeb5a9330aba57bc7472))

## 2021-03-27

- Added Xiao's and Goro's Queen dialogues. ([`e600f34`](https://github.com/luni/DarkCloud-Enhanced/commit/e600f34186b2d0819db61cd0caec011bef9fcc8d))

## 2021-03-28

- Finished Queen's Dialogues (Some dialogues remain to be tested/rechecked) ([`558317a`](https://github.com/luni/DarkCloud-Enhanced/commit/558317a623113fe3bbd758071a2f72a5073b37ce))

## 2021-03-30

- Finished 1st revision of the allies Queen dialogues ([`f2aa489`](https://github.com/luni/DarkCloud-Enhanced/commit/f2aa489d87f2544a962520e668be9eb8defdee11))
- Added correct Xiao's camera angle for Queens ([`3d67b3a`](https://github.com/luni/DarkCloud-Enhanced/commit/3d67b3ad78dd4db76d4cdee96cd0bbcef435aff8))

## 2021-04-04

- Added 100% collection check dialogue in brownboo (and updated PNACH file to have it work) ([`dffeff2`](https://github.com/luni/DarkCloud-Enhanced/commit/dffeff28f23b2fcb90318eb5dcba3162e4e55cd6))

## 2021-04-07

- Added Miniboss message trigger + display. ([`27450a0`](https://github.com/luni/DarkCloud-Enhanced/commit/27450a088501832f6f4de567999ac15fa5dab9ed))
- Weird meme test ([`d4e0ef1`](https://github.com/luni/DarkCloud-Enhanced/commit/d4e0ef132bcd00482ee17758f72f3289b3184e0b))

## 2021-05-09

- Plenty of changes, mainly about the sidequests + fixed WriteByte and added WriteOneByte ([`0951640`](https://github.com/luni/DarkCloud-Enhanced/commit/0951640be09d5521c6ba3bcc6ef8d90c3142fdcd))
- Minor code clean up ([`7543d64`](https://github.com/luni/DarkCloud-Enhanced/commit/7543d64b2eadf466c32fec2c61eb8ca21975f21b))

## 2021-05-12

- Added first working sidequest (macho), added dialogue system to muska racka, changed Dayuppy's dungeonmessage to start a new separate thread and custom displaytime for it ([`d53b611`](https://github.com/luni/DarkCloud-Enhanced/commit/d53b611f6fedccca1580c79c1fff94d3f232f2c2))
- Muscka Xiao dialogue finished (w/o Ungaga & Theo) ([`f148e9e`](https://github.com/luni/DarkCloud-Enhanced/commit/f148e9e9e9923e96a8efca530c7dd66431ee2dff))

## 2021-05-14

- Added 2nd monster quest (matataki, Gob) ([`9947b71`](https://github.com/luni/DarkCloud-Enhanced/commit/9947b715ceff9b030900aaf1f5e4d5c521edf237))
- Goro Muska Lacka dialogue done (w/o Ungaga & Theo) ([`d8aded3`](https://github.com/luni/DarkCloud-Enhanced/commit/d8aded3dd0d7b840174718f20a8dd70d8684ab97))

## 2021-05-15

- Implemented Goro's new Muska Racka dialogues. ([`a5901e3`](https://github.com/luni/DarkCloud-Enhanced/commit/a5901e3acd3483f2b6e12f8437b4f3593a34a5d9))

## 2021-05-17

- Added Jack's monster quest (and sidequest dialogues to queens) ([`bcec5dd`](https://github.com/luni/DarkCloud-Enhanced/commit/bcec5ddaeafe2c2220e9ec2cae33b33ce3c244d6))

## 2021-05-18

- Added Chief Bonka's monster side quest (and sidequest system in Muska racka) ([`bf56c4a`](https://github.com/luni/DarkCloud-Enhanced/commit/bf56c4a3d2b793dcd2328fbb1c4ee049e416fe9d))
- Ruby's Muska Racka dialogue done (w/o Ungaga and Theo) ([`66f76d7`](https://github.com/luni/DarkCloud-Enhanced/commit/66f76d70ac33da8ca1756d6191a76dc659ada9db))
- Ungaga Muska Racka dialogue done (w/o easter egg dialogue) ([`e2ed9e4`](https://github.com/luni/DarkCloud-Enhanced/commit/e2ed9e4aa3197ffe203320729b7fd3c0e90b9be4))

## 2021-05-21

- Osmond Muska Racka dialogue done (w/o Ungaga and Theo) ([`22a4a04`](https://github.com/luni/DarkCloud-Enhanced/commit/22a4a04f4c7e0f4b138206e8019e5d13ae0ca1a3))
- Updated Osmonds Muska Racka dialogue with a correction. ([`b1f58a4`](https://github.com/luni/DarkCloud-Enhanced/commit/b1f58a49de8e5b338c8083df1123ddb457a9f57a))

## 2021-05-24

- Added new stat addresses to the enemy and miniboss files. ([`252a3d1`](https://github.com/luni/DarkCloud-Enhanced/commit/252a3d1d5117f823a5ef95feb06810d92af7b159))

## 2021-05-26

- Minor updates to the miniboss feature. ([`da19076`](https://github.com/luni/DarkCloud-Enhanced/commit/da19076ac4b251513e06779e9cad0c7f9938b345))

## 2021-05-28

- Added first Fishing Quest in Norune ([`9f3abe7`](https://github.com/luni/DarkCloud-Enhanced/commit/9f3abe7a3a7a4e7b7fa747f0f6bd7bb038d9f9bc))

## 2021-05-29

- Added dialogues for Theo and Ungaga at Sun&Moon Entrance ([`74f853e`](https://github.com/luni/DarkCloud-Enhanced/commit/74f853e7119cf3627e7d05f9dc840adcada7e4e8))
- Updated MinibossThread ([`1f0673a`](https://github.com/luni/DarkCloud-Enhanced/commit/1f0673aa684b40b8ad0a4fa1991b9442dbee3b97))

## 2021-05-30

- Update Miniboss 2 ([`7451847`](https://github.com/luni/DarkCloud-Enhanced/commit/74518471a9e8252273eb56162d02237e7ff0bf08))

## 2021-05-31

- Added second Fishing Quest type in Norune ([`c1a0364`](https://github.com/luni/DarkCloud-Enhanced/commit/c1a0364e10fb5605437b80f61ae515ac60819542))

## 2021-06-03

- Fishing Quest 1 added in Matataki ([`f97ded2`](https://github.com/luni/DarkCloud-Enhanced/commit/f97ded2ba70b11636f10ed0e327f9ead2545012a))
- Almost done with mini boss! ([`8f9435b`](https://github.com/luni/DarkCloud-Enhanced/commit/8f9435b7cbe48fbf172ba9f57d6fd9c97e0d2769))

## 2021-06-07

- Planning of the message structure. ([`b6ec197`](https://github.com/luni/DarkCloud-Enhanced/commit/b6ec19783a31663fe45b94898430f8fac3dfabf3))
- Fishing Quest 2 added in Matataki ([`311a81b`](https://github.com/luni/DarkCloud-Enhanced/commit/311a81bd75bf2b738f3918b744f0cee92f80ce6f))
- Fixed the missing arguments on MiniBossSpawn call within Dungeon.cs ([`8777345`](https://github.com/luni/DarkCloud-Enhanced/commit/8777345e67db2325da717546832a613b08be0f73))

## 2021-06-10

- Added custom dialogue system to Yellow drops ([`dea1166`](https://github.com/luni/DarkCloud-Enhanced/commit/dea11664a1e4ca3c66143cbcf04cd27ee68b95e9))

## 2021-06-12

- Miniboss feature done! ([`1ceb7a7`](https://github.com/luni/DarkCloud-Enhanced/commit/1ceb7a78ebe1d8bed0e90872d67574ee2473f681))

## 2021-06-14

- Added both Fishing Quests in Queens + special with fish visibility reward ([`0ae4fa4`](https://github.com/luni/DarkCloud-Enhanced/commit/0ae4fa482e1f99de1748ece6548900e2eaf697db))

## 2021-07-29

- Added Fishing Quests in Muska Racka ([`4fdb823`](https://github.com/luni/DarkCloud-Enhanced/commit/4fdb8236450de473a09c1feadc24798782f035cf))

## 2021-08-23

- Matataki and Yellow Drops dialogues finished and tested. ([`fbe0305`](https://github.com/luni/DarkCloud-Enhanced/commit/fbe0305a2e5babbeadb92c5f6c2f174b5bfb5614))

## 2021-08-24

- Updated Xiao and Ungaga Yellow Drop dialogues with corrections. ([`d0e10cb`](https://github.com/luni/DarkCloud-Enhanced/commit/d0e10cb9b2ec2c3e1afe00f02f7bd52f3184177e))
- Angel Gear custom effect started process ([`00f0f28`](https://github.com/luni/DarkCloud-Enhanced/commit/00f0f283157ead6c27e983f7f78f9674d40ad718))

## 2021-08-25

- Added verifications on Angel Gear effect; Updated some Player functions; Added Weapon class; ([`4e8ae16`](https://github.com/luni/DarkCloud-Enhanced/commit/4e8ae1699550b86276d98d480fc97cce5f81c34e))

## 2021-08-26

- Added validations for Angel Gear effect and updated some methods ([`83ee999`](https://github.com/luni/DarkCloud-Enhanced/commit/83ee9994c3efd2201f168696548aec34a79089af))

## 2021-08-29

- Added the isBypassBoneDoor address. ([`73568e2`](https://github.com/luni/DarkCloud-Enhanced/commit/73568e20a4965df6019d0992f5f790e528c3d9ac))

## 2021-08-30

- Bone Rapier effect finished; ([`50e6d5f`](https://github.com/luni/DarkCloud-Enhanced/commit/50e6d5fd64a7204fedde73b79ff42775757068fc))

## 2021-08-31

- Fixed bone rapier effect for when Toan changes weapons; ([`be23338`](https://github.com/luni/DarkCloud-Enhanced/commit/be233382501366cc45e1612271a8f1bf25a3c75c))

## 2021-09-05

- Attempt to fix the miniboss flying meme ([`ea97600`](https://github.com/luni/DarkCloud-Enhanced/commit/ea97600bf12581c98b62009f5199c22ae887186b))
- Fix attempt on the flying miniboss meme ([`fdc9617`](https://github.com/luni/DarkCloud-Enhanced/commit/fdc96173f54c3fa00c07c385657d9ee8d8ad0059))

## 2021-09-08

- Lamb sword buff complete + minor change to the bone rapier door message ([`f9ea1b7`](https://github.com/luni/DarkCloud-Enhanced/commit/f9ea1b7075e695885765fd66fc98a0365182cf8a))

## 2021-10-11

- - Added full items and enemies IDs list; - Added various functions to query inventories (except storage); - Added various functions to set items in the players inventory (Bag, Weapons, Attachments); ([`7eefd3f`](https://github.com/luni/DarkCloud-Enhanced/commit/7eefd3fbcfe407dbdae6813aa738278c6c1396aa))

## 2021-10-31

- - Updated inventory management functions; - Finished 7th Heaven custom effect; ([`a2afb84`](https://github.com/luni/DarkCloud-Enhanced/commit/a2afb84d67db8174dd907d2577e9ad7db1588b3b))

## 2021-11-04

- - Finished the Star Breaker custom effect; - Updated the player weapons with more addresses; - Added a GetEnemiesKilledIds function; - Fixed some issues on the inventory management functions; ([`420ad29`](https://github.com/luni/DarkCloud-Enhanced/commit/420ad29acba615b82c2992a98d6a7b2de33caca4))

## 2021-12-29

- - Added Secret Armlet custom effect; - Added Magic circle addresses; ([`b977628`](https://github.com/luni/DarkCloud-Enhanced/commit/b9776282d73b5018f7caaeddf263d20f624c8232))

## 2021-12-31

- - Added multiple cheats and fixed the broken dagger glitch ([`0c58521`](https://github.com/luni/DarkCloud-Enhanced/commit/0c5852165bcee3beab49cf1ab71f46323d667f65))
- - Added another cheat to unlock floors ([`1123731`](https://github.com/luni/DarkCloud-Enhanced/commit/1123731fa2e09a4b2555534a943e966f66fe67be))
- - Added Custom Dialogue system to Brownboo Village ([`b9680b1`](https://github.com/luni/DarkCloud-Enhanced/commit/b9680b1d5935e9eab0eca827f7432b4cfef891d5))

## 2022-01-02

- - Fixed Ungaga door animation (shortened animation time) ([`197ee0d`](https://github.com/luni/DarkCloud-Enhanced/commit/197ee0dca2a375c6b5d04af82e556ce0200b8bf5))

## 2022-01-03

- - Implemented first part of Master Fishing quest (tracking all caught fish) ([`26cdce5`](https://github.com/luni/DarkCloud-Enhanced/commit/26cdce54bf46b996407738ed418ada4817493e22))
- - Finished Master Fishing Quest ([`ba614c9`](https://github.com/luni/DarkCloud-Enhanced/commit/ba614c95b551298b3ca3288b211cefb86141fc12))

## 2022-01-04

- - Implemented the Item Fetch quest (only for Norune) ([`62df58e`](https://github.com/luni/DarkCloud-Enhanced/commit/62df58e442d3fb4040e64e6bbca35322364a74d1))

## 2022-01-05

- - Attempts to fix the mini boss memes; - Removed the console logs for the bag attachment functions; ([`7842e9a`](https://github.com/luni/DarkCloud-Enhanced/commit/7842e9aa13a8f1122c3500836c73d4b4f926805d))
- - Added Matataki, Queens and Muska Racka item fetch quests, and hopefully fixed the clown memes ([`3148443`](https://github.com/luni/DarkCloud-Enhanced/commit/31484438d8355772f18160e094b680f6149d0665))

## 2022-01-07

- - Finished Demon Shaft item quest, added flame key as 100% reward, added dialogue options to Yellow Drops ([`ee54978`](https://github.com/luni/DarkCloud-Enhanced/commit/ee54978b805dc207ec9f94507843f239ae7a40df))

## 2022-01-08

- - Added 2 sidequests in Yellow Drops with Map and Magical Crystal rewards ([`75e60c6`](https://github.com/luni/DarkCloud-Enhanced/commit/75e60c62a446f9c131eb1e59c9fcb753ba0acb82))
- - Added Map and Magical Crystal items functionality ([`69adc85`](https://github.com/luni/DarkCloud-Enhanced/commit/69adc8599e6f1c86f065bae9726b890c82ebcbfb))

## 2022-01-10

- - Added Brownboo Dialogues & fixed couple minor things ([`ea3edaa`](https://github.com/luni/DarkCloud-Enhanced/commit/ea3edaa31bb748589223a9b8e542f989ed3ac720))

## 2022-01-11

- - Started on Mayor's sidequests ([`43908bd`](https://github.com/luni/DarkCloud-Enhanced/commit/43908bd003f6e45562ce3784cca1417c10ac2393))

## 2022-01-12

- - Pretty much finished Mayor's sidequest ([`9643c79`](https://github.com/luni/DarkCloud-Enhanced/commit/9643c790b3cd6781b00ef9a4430ece9eadb384d3))

## 2022-01-13

- - Added repair powder + escape powder functionality & made some powders stackable ([`4c60e86`](https://github.com/luni/DarkCloud-Enhanced/commit/4c60e86583d0c4a6f874c08d9106c9ec003a566c))

## 2022-01-14

- - Added Fairy King dialogues ([`3a9125f`](https://github.com/luni/DarkCloud-Enhanced/commit/3a9125fe81bde32c8f6429ddbd89bd70aa44ea44))

## 2022-01-22

- - Added more addresses; - Small corrections to the bag functions; ([`d479915`](https://github.com/luni/DarkCloud-Enhanced/commit/d47991570475a1df4494da676fd7559ce44da8a4))

## 2022-01-23

- - Added Mardan Swords effects ([`a538954`](https://github.com/luni/DarkCloud-Enhanced/commit/a53895477797f2093f6d73e39862742fb53788e3))

## 2022-01-27

- - FIXED RUBY ELEMENT MEME! ([`0706f9f`](https://github.com/luni/DarkCloud-Enhanced/commit/0706f9f549e3c2debc0ace3c17b797cbbab7b1da))
- - More fixes on ruby element + some changes on DisplayMessage ([`01f8feb`](https://github.com/luni/DarkCloud-Enhanced/commit/01f8febc0fe2c2738f85ad6b86bcddc4655229b4))
- Added Internal Embedded Resources for Ruby Meme Fix. ([`cd65cc0`](https://github.com/luni/DarkCloud-Enhanced/commit/cd65cc047440f1adf9da190a8fa881af492e1ffe))
- Updated Dayuppy.cs to initialize resource loading for Ruby Meme Fix. ([`5e60906`](https://github.com/luni/DarkCloud-Enhanced/commit/5e60906b2625c2d4b4006a3fd458e8fe05935176))
- - Changed PNACH resource file to never copy ([`ad49576`](https://github.com/luni/DarkCloud-Enhanced/commit/ad4957613b39480d0729eca8a8dacd5d9767379e))

## 2022-01-28

- - Fixed active item icons not disappearing & fixed couple of things with allies in towns ([`794e869`](https://github.com/luni/DarkCloud-Enhanced/commit/794e8697bac6924b236441e577d7ed5cf62a5651))

## 2022-02-02

- - Added clock/time advancement to Yellow Drops + Dark Heaven (+ updated PNACH) ([`813a6bc`](https://github.com/luni/DarkCloud-Enhanced/commit/813a6bc8099f4c127d64026806d73885c3585619))

## 2022-02-04

- - Improved Town Dialogue speed ([`9edec63`](https://github.com/luni/DarkCloud-Enhanced/commit/9edec63f4571b54bfb73d9812055eebf536171c0))

## 2022-02-13

- - Added Sword of Zeus & Inferno custom effects ([`b879cfe`](https://github.com/luni/DarkCloud-Enhanced/commit/b879cfe9488fb5a9ae525cdeb36223c4d179d32f))

## 2022-02-18

- - Added the Chronicle Sword effect and bunch of addresses ([`753c9e1`](https://github.com/luni/DarkCloud-Enhanced/commit/753c9e16f0a87fa1eaec7e15f4320b3e13775cf3))

## 2022-02-26

- - Added a queue system for the custome messages; ([`e35b682`](https://github.com/luni/DarkCloud-Enhanced/commit/e35b682ce146d2c572966922929b07496987f89e))

## 2022-02-27

- - Added a check on the message queue to ignore the "Thirst reached its limit" message; - Updated the element swap message to dynamically change its width according to the selected element word; ([`31b2afd`](https://github.com/luni/DarkCloud-Enhanced/commit/31b2afd10e18c98a78b7f5ab6b92f1c582558eb9))
- - Update to the message display; ([`d4b3450`](https://github.com/luni/DarkCloud-Enhanced/commit/d4b3450a6a2f1101f8ccd39952256e51fac5ea8d))

## 2022-02-28

- - Fixed the message display size; ([`07494ca`](https://github.com/luni/DarkCloud-Enhanced/commit/07494cace837dd58f8bddc03bb37fede5c97ed34))

## 2022-03-03

- - Finished Chronicle Sword effect, added Damage Source check to ReusableFunctions, applied big chest chance modifiers, fixed Matador and changed mod UI to have option for normal user and dev ([`7520623`](https://github.com/luni/DarkCloud-Enhanced/commit/752062359bc13f9c11196464523da7c18237fe73))

## 2022-03-05

- - Added "It's finished" dialogues to Norune ([`8274b50`](https://github.com/luni/DarkCloud-Enhanced/commit/8274b50ebb183571ee6583d9509c233851defd1e))

## 2022-03-08

- - Added matataki "its finished" dialogues and worked on the "user-mode" and added conditions and stuff to it ([`a2f0dbf`](https://github.com/luni/DarkCloud-Enhanced/commit/a2f0dbfa1cd33cf085146e39008972356783af7c))
- - Fixed Ungaga's weapon table weapon changes to affect endurance instead of magic; ([`44c58a4`](https://github.com/luni/DarkCloud-Enhanced/commit/44c58a425c0e7e89299005c72ab82a0f2bd4db1b))

## 2022-03-09

- - Fixed a bug in town character that prevented the thread to run (infinite while loop for Yellow Drops clock); ([`a55b8d8`](https://github.com/luni/DarkCloud-Enhanced/commit/a55b8d85d13e1630f0ec568333c9162397aac2d9))

## 2022-03-10

- - Improved MainMenuThread, added couple options to usermode, added rest of the "its finished" dialogues, improved some of the sidequest dialogues ([`df064e2`](https://github.com/luni/DarkCloud-Enhanced/commit/df064e2ec25ff76de2ee794c21b78a43fc1ec08b))

## 2022-03-13

- - Added Daily Item Rotation to the shops, added more Mod Options and improved MainMenuThread ([`4d8d4bc`](https://github.com/luni/DarkCloud-Enhanced/commit/4d8d4bc7bd5c08eb57cece50fdac5d41eaed8870))

## 2022-03-14

- - Added Quit option to usermode, fixed Ungagas bomb throwing animation, fixed Toan references in dialogues ([`596785e`](https://github.com/luni/DarkCloud-Enhanced/commit/596785e3a02fa6e83f5e96a5b4edf65f15b818a8))

## 2022-03-16

- - Updated all the weapon new buy and sell prices; - Added a new buildup route to Skunk (to G Crusher); ([`f74b921`](https://github.com/luni/DarkCloud-Enhanced/commit/f74b9212410bc280b8df3b5f6923fc5b81481bd7))

## 2022-03-17

- - Added the remaining item price updates; ([`d747142`](https://github.com/luni/DarkCloud-Enhanced/commit/d747142df9c57ff3924ee177f28c4daa28a6bd77))

## 2022-03-20

- - Minor changes to labels, descriptions and value fetching; ([`9eea558`](https://github.com/luni/DarkCloud-Enhanced/commit/9eea558fd1f79a7577c23b75e244c05b5f4254c8))

## 2022-03-23

- - Updates to the form window; ([`21a85ac`](https://github.com/luni/DarkCloud-Enhanced/commit/21a85ac37e7407258560fa1e4ecdeccdcd2829e7))

## 2022-03-24

- - Updates to the form window; ([`1765d5c`](https://github.com/luni/DarkCloud-Enhanced/commit/1765d5cc6fe76af18bfa360e621fc74936147af7))

## 2022-03-26

- - Updates to the mod window; - Converted the Player.Gilda methods to static; ([`3c87a99`](https://github.com/luni/DarkCloud-Enhanced/commit/3c87a990ee26edf3c14c189846401553b9082f6e))
- - Reviewed most custom effects (missing on super nova, starbreaker and chronicle 2) and applied corrections; - Changed the default DisplayMessage timeout from 5000ms to 8000ms as discussed previously; - Further improvements to the mod window, added some validations; - Added a new address to Player.cs (animationId); - Added 2 new methods to Player.Ruby (IsChargingAttack and IsReleasingChargeAttack); - Added a new method on ReusableFunctions.cs (AwaitUnpause) in order to halt current ongoing threads and wait for the user to unpause the game and resume the processes; ([`0c22187`](https://github.com/luni/DarkCloud-Enhanced/commit/0c22187c543abe75267cafcdb32ca02b25c7bfd6))

## 2022-03-27

- - Added special effect rerolling for weapons and mayor sidequest backfloor item to fairy king shop ([`a228ae3`](https://github.com/luni/DarkCloud-Enhanced/commit/a228ae326486e424c2823284c05bd99b6fc6a136))

## 2022-03-29

- - Updated the mini boss process and fixed some issues; ([`69227b1`](https://github.com/luni/DarkCloud-Enhanced/commit/69227b192e9e398c7f45afe124ba50e7b2ebd104))
- - Updated the mod window; - Updated addresses descriptions; - Properly implemented the SynthSphere menu listener thread; - Applied the correct verifications on the thread mod window buttons to prevent crashing when starting a second thread; ([`36008a6`](https://github.com/luni/DarkCloud-Enhanced/commit/36008a6d1e8a58afae9bd1cd7ad2275bd7f570aa))
- - Added the option to leave dungeon at floor selection ([`f071629`](https://github.com/luni/DarkCloud-Enhanced/commit/f07162959b7ec34ad20bde949ce80c9811d6f103))
- - Floor message clear update attempt 1 ([`e1ebdee`](https://github.com/luni/DarkCloud-Enhanced/commit/e1ebdee10c067ad6da4e2c27e0fb5805d08761ac))
- - Merge meme commit ([`f93af5e`](https://github.com/luni/DarkCloud-Enhanced/commit/f93af5e5f5ae7bd7c7d6f7f13a6eaa71ff92f073))

## 2022-04-02

- - Fixed a Synth Sphere thread bug where the effect would stop working upon leaving the correct menu; - Updated the Element Swapping trigger conditions; - Fixed some minor bugs in the code; ([`0cc6f46`](https://github.com/luni/DarkCloud-Enhanced/commit/0cc6f4640bcfbd327fd76de5cf88cd2da27f8dcb))

## 2022-04-03

- - Added timestamp to all console logs; - Fixed the inventory functions to account for yellow slots and other bugs; - Set the proper roll chances; - File cleanup; ([`5e1b941`](https://github.com/luni/DarkCloud-Enhanced/commit/5e1b9412f75a4be92f5fa973f7639914d6c20ec1))
- - Added console logging to a text file, added .exe icon ([`323033b`](https://github.com/luni/DarkCloud-Enhanced/commit/323033b11a4f0294555e5edc131d146d4c1f1742))
- - Fixed Fairy King's dialogue about allies ([`f40f0fa`](https://github.com/luni/DarkCloud-Enhanced/commit/f40f0faf0f2c29f8ae7d37efcddc346cccd9a73b))

## 2022-04-07

- - FIRST VERSION OF BETA TESTING - Changed the savefile check and a minor change in ModWindow ([`6d36634`](https://github.com/luni/DarkCloud-Enhanced/commit/6d36634347388ea08d59647913860f6b7a8d417f))

## 2022-04-15

- - Added proper saving to credits & demon shaft unlocking ([`cd3a8d0`](https://github.com/luni/DarkCloud-Enhanced/commit/cd3a8d0c092ca5a8846c868c02d1f2b31d1a8286))

## 2022-04-20

- - Applied the remaining Xiao dialogue camera angles; - Fixed some bugs in the ally dialogues; ([`5339de1`](https://github.com/luni/DarkCloud-Enhanced/commit/5339de1fb5a31895370c989cc48ccda62c1bbc8b))
- - Added heart symbol to dialogues ([`46ee66e`](https://github.com/luni/DarkCloud-Enhanced/commit/46ee66ed88dbf249944123fa46420ba6ad8e5f0c))

## 2022-05-08

- -Added option to disable character attack sounds -Added new text/dialogue to the prologue -Fixed a bug with element quick-swapping not including attachment elements -Made repair&escape powder active items usable when near enemy -Made the 1st monster quest in norune always start with Dashers ([`0983e91`](https://github.com/luni/DarkCloud-Enhanced/commit/0983e91c964e98a82445d698697c927f3c4871fc))

## 2022-05-12

- -Removed some required items from the item collection quest -Fixed couple bugs with 7th Heaven -Fixed a bug with element swapping, reloading save for 2nd time made a 2nd instance of the thread -Fixed a bug with Miniboss which caused the gatekey to not be swapped correctly -Fixed a fishing rod appearing in fairy king shop ([`3768252`](https://github.com/luni/DarkCloud-Enhanced/commit/3768252ae8cf714551ac47180bd5a09bec399d53))

## 2022-05-15

- - Added dark cloud font and applied to the mod window; ([`a0b9dd1`](https://github.com/luni/DarkCloud-Enhanced/commit/a0b9dd12c5dadd43513a86262ab4bc49b6090ffa))
- -Mod launches to usermode ([`788e041`](https://github.com/luni/DarkCloud-Enhanced/commit/788e041d2ae5200adb3ddd88f8eadc7d6d0f57f4))

## 2022-05-16

- -Added credits page -Options are now saved when saving the game ([`ca374a7`](https://github.com/luni/DarkCloud-Enhanced/commit/ca374a7daeb1f30bd81d97f69fd7086ffb53beb2))

## 2022-05-18

- -Some bugfixes ([`9e2b954`](https://github.com/luni/DarkCloud-Enhanced/commit/9e2b954bf498b73db446f27fa458fa85b4b7e61c))

## 2022-05-19

- -More bugfixes ([`607890c`](https://github.com/luni/DarkCloud-Enhanced/commit/607890ceed5bdb38d5f53cf73973cf56eaa04570))

## 2022-05-20

- - "HornHead" bug fix; - Added the Item Message prompt ID address; ([`e410435`](https://github.com/luni/DarkCloud-Enhanced/commit/e41043539ac47fc2a496f49ac8b2095419bee886))

## 2022-05-22

- - Fixed an unintended interaction where Angel Gear would revive characters with its effect; - Fixed some typos; - Reworked a couple dungeon message texts; - Removed and altered some logs; ([`422a390`](https://github.com/luni/DarkCloud-Enhanced/commit/422a39091bc6fe18f573a4dfd0f9d1d7e1a0c91d))

## 2022-05-25

- -Fixed a bug with Secret Armlet's ability not working properly in the backfloors -Fixed a bug where Osmond was unable to quick-swap to "None" element with machine guns -Fixed a bug where the DBC monster kill quests were completeable in Demon Shaft -Sil and Gol can no longer be rolled as the Special enemy in Demon Shaft, since they are unable to drop items. -Fixed a bug which caused base weapon stats being increased multiple times to Ungaga's and Osmond's weapons -Another bugfix to the "HornHead" message (maybe for good this time!) ([`b321e61`](https://github.com/luni/DarkCloud-Enhanced/commit/b321e61754a702f9b4cd3b25a55ba76ea062483c))

## 2022-05-30

- -You can now loop the element quick-swapping (from None to Fire and vice versa) -Fixed a bug where the mystery circle effect "Monster is pumped with energy" would reduce the stamina timer of a Special enemy -Fixed a bug involving Chronicle Sword's ability and Mimics -Fixed a bug where Chronicle 2's ability would stop working after levelling it up ([`fd7f6eb`](https://github.com/luni/DarkCloud-Enhanced/commit/fd7f6eb3908baa95815784951d3a7c4bd5355863))

## 2022-06-08

- - Minor dialogue name fixes; ([`fa78ba8`](https://github.com/luni/DarkCloud-Enhanced/commit/fa78ba884397fbe6d462d81c8e85141eb212b6de))

## 2022-06-15

- - Added an option to mute all music - Attempt to fix a bug with minibosses staying small (added 200ms sleep to miniboss) - Fixed a bug involving Mayor's quests ([`6d10bb1`](https://github.com/luni/DarkCloud-Enhanced/commit/6d10bb1039eeaf4bed9c66bb785559bf8dc53373))

## 2022-06-20

- - Minor dialogue text corrections; - Decreased the drop chance of backfloor key from minibosses; - Decreased the chance of the backfloor keys appearing in the daily rotation in shops; - Increased the prices of Tram Oil, Sundew, Flapping Fish, Secret Path Key, Bravery Launch and Flapping Duster; - Fixed an issue where Rondo's shop would display Gaffer's instead when accessing it with allies other than Toan; - Added more coments and organized some values; ([`328be04`](https://github.com/luni/DarkCloud-Enhanced/commit/328be04bfeff33665331e566789c33b8f1b437b6))

## 2022-06-23

- - Added a button to submit bug reports ([`00de3b9`](https://github.com/luni/DarkCloud-Enhanced/commit/00de3b9677d17cff281bd0328cd1d7aaa633d922))
- - More dialogue typos fixed; ([`4ac4f2e`](https://github.com/luni/DarkCloud-Enhanced/commit/4ac4f2ee8a4ab0d2a3064bf94bc085571b03c5a0))
- - Added a dialogue requested by beta tester ([`1903b1a`](https://github.com/luni/DarkCloud-Enhanced/commit/1903b1aa48d8e4f06b0996bdb7318616f69d3ef5))

## 2022-06-28

- - typos ([`625b5e8`](https://github.com/luni/DarkCloud-Enhanced/commit/625b5e894d0a684d6f26cef906b9d5a37598ccfc))
- - More typos fixed; ([`4bce1b5`](https://github.com/luni/DarkCloud-Enhanced/commit/4bce1b5775ad5c54db68a008adecfa4e1b45d557))

## 2022-07-01

- - Added extra dialogue and a label change ([`3949f6e`](https://github.com/luni/DarkCloud-Enhanced/commit/3949f6e6f785e47b33fdf5583e128e774693f29b))
- - Added discord button ([`8cb3cb4`](https://github.com/luni/DarkCloud-Enhanced/commit/8cb3cb45a94ce3dfa06ac3450893d59c45c9528e))
- - Added discord button ([`8685c7a`](https://github.com/luni/DarkCloud-Enhanced/commit/8685c7a6a5152b748799f02544946eaec2bca360))

## 2022-06-30

- - Dialogue fixes; - Increased the proc change for Hercule's Wrath from 15% to 30%; - Increased the proc chance for Babel Spear from 4% to 6%; ([`b536991`](https://github.com/luni/DarkCloud-Enhanced/commit/b5369919bf1e1e74d93e0373fdbd672abe6155b1))
- - Dialogue fixes; - Increased the proc change for Hercule's Wrath from 15% to 30%; - Increased the proc chance for Babel Spear from 4% to 6%; ([`4bcc039`](https://github.com/luni/DarkCloud-Enhanced/commit/4bcc0395bcb13540185102219bd19fe16a7cd92f))
- - Dialogue fixes - Increased the proc change for Hercule's Wrath from 15% to 30% - Increased the proc chance for Babel Spear from 4% to 6%; ([`770dba4`](https://github.com/luni/DarkCloud-Enhanced/commit/770dba4daae50ed53e0c7011a4595c5f258299cd))

## 2022-07-05

- -Changed spawn check to use enemy 15 instead of enemy 1 (possibly fixing a bug with gatekeys in wise owl) - Fixed a bug with miniboss's stamina not resetting after visiting backfloor ([`96999f7`](https://github.com/luni/DarkCloud-Enhanced/commit/96999f7da4452d828c90859a74a449dd18406581))
- -Changed spawn check to use enemy 15 instead of enemy 1 (possibly fixing a bug with gatekeys in wise owl) - Fixed a bug with miniboss's stamina not resetting after visiting backfloor ([`6fa334a`](https://github.com/luni/DarkCloud-Enhanced/commit/6fa334ae6dbb43a076332b89b218870820e74011))
- - Minor re-works and added comments; ([`c1b45c9`](https://github.com/luni/DarkCloud-Enhanced/commit/c1b45c9f350d140ea5c71a4a82695ef0c144ec60))
- - Minor re-works and added comments; ([`43e2668`](https://github.com/luni/DarkCloud-Enhanced/commit/43e26689e7241bc5b553a58e6f482ecb9c6ad033))

## 2022-10-19

- Updated to support 65-bit pcsx2. Cleaned up a few old functions. Removed Owin packages? Did some other things. ([`89f75cb`](https://github.com/luni/DarkCloud-Enhanced/commit/89f75cb260ff4997da5b9793b576f6f3c84a3a1e))
- Updated offsetreader.exe and associated function to retrieve EEMem pointer.. ([`4b91cac`](https://github.com/luni/DarkCloud-Enhanced/commit/4b91cac5b027a8114f17c1c41e546f2cddecb1d4))

## 2023-03-29

- Updated to be compatible with latest nightly builds ([`fab2efd`](https://github.com/luni/DarkCloud-Enhanced/commit/fab2efd71123ee93134f1e7b3309653d71e5bbf8))
- Added pcsx2_offsetreader solution ([`1fd50cf`](https://github.com/luni/DarkCloud-Enhanced/commit/1fd50cfcf03111f25c5cf7b72e477aaf153791d0))

## 2023-04-17

- Bug fixes: - Fixed a code softlock with Macho's sidequest - Fixed cheat code thread duplicating after resetting game - Fixed Ruby's element swapping breaking after resetting game ([`1e94f23`](https://github.com/luni/DarkCloud-Enhanced/commit/1e94f234930480153742a1522a5ad23d796975b8))

## 2023-06-01

- - Fixed a new dungeon process starting when leaving a dungeon (yellow drops crash fix) - Fixed Demon Shaft Backfloor Clowns not giving correct items - Disabled "Improved Graphics" option if using Nightly version ([`527da01`](https://github.com/luni/DarkCloud-Enhanced/commit/527da01a5ac5dbf5c3ae2ab06fba869aba692b85))

## 2023-06-17

- - Code cleanup on Mike's side; ([`1380d14`](https://github.com/luni/DarkCloud-Enhanced/commit/1380d14318d1496befc1f94fc2c3674e04dffb7f))

## 2023-08-23

- This is the preparation for the official release, possibly the last edit for this repository. A new Github project will be created. ([`61135dd`](https://github.com/luni/DarkCloud-Enhanced/commit/61135dd0233e7cdb139d642a7d8c59f0398b1e7b))
- Removed some useless files ([`4d924dc`](https://github.com/luni/DarkCloud-Enhanced/commit/4d924dc88d8b25354364a702cf5bd726df6b2197))
- Create LICENSE ([`e3337e2`](https://github.com/luni/DarkCloud-Enhanced/commit/e3337e2788333a89c9b41390a6493ec8851864f2))

## 2023-08-24

- Create README.md ([`a2ab70c`](https://github.com/luni/DarkCloud-Enhanced/commit/a2ab70c8405d089ed28f1f42765ef7014f13a3d0))

## 2026-01-01

- Memory API refactor and cleanup ([`0f20c74`](https://github.com/luni/DarkCloud-Enhanced/commit/0f20c745c55caa4b36fc65bcce4b0faacc72a5cd))

## 2026-07-25

- Add PAL region support and translated SCES-50295 pnach. ([`8c702b3`](https://github.com/luni/DarkCloud-Enhanced/commit/8c702b360d410ca8713560cad155eaa9021198b9))
- Add Linux compatibility layer and migrate changelog PDF to Markdown. ([`8703462`](https://github.com/luni/DarkCloud-Enhanced/commit/87034624153a1b857bd22211ecdca409ce2d4aad))
- Harden Linux memory I/O in Platform.cs. ([`96e4b80`](https://github.com/luni/DarkCloud-Enhanced/commit/96e4b80f26afcffa5868d914b2e306460cf6f533))
- Update pal_upgrade.md with PDF and Linux status. ([`8cb12f3`](https://github.com/luni/DarkCloud-Enhanced/commit/8cb12f3164e8b47065051954b20658d088a731d1))
- Use typed discards for Platform read/write/protect out parameters. ([`395e8a4`](https://github.com/luni/DarkCloud-Enhanced/commit/395e8a4af075148485972dfc3d6b4bf13a350689))
- Add GitHub Actions CI/CD workflow for automated releases. ([`8bb72ec`](https://github.com/luni/DarkCloud-Enhanced/commit/8bb72ece4e6594a4d4860a00471b3dc346c10265))
- Fix Linux/Mono build and verify compilation with xbuild. ([`fc935e3`](https://github.com/luni/DarkCloud-Enhanced/commit/fc935e3ff4ceed10176ff69032d1915933f708bc))
- Add Mono/Linux CI job and document local Mono verification. ([`b4a3739`](https://github.com/luni/DarkCloud-Enhanced/commit/b4a37395b5af70e92cdfb429f6c2f0aab1f35863))
- Use native PCSX2 ELF exports on Linux for EEmem discovery. ([`4e4c1f9`](https://github.com/luni/DarkCloud-Enhanced/commit/4e4c1f91105821d9774e10079f8be439e225a0cb))
- Fix PAL boot string mapping used by region detection and boot check. ([`fa1293f`](https://github.com/luni/DarkCloud-Enhanced/commit/fa1293f314cd468ff603e6afdd2d8df6c7893e1a))
- Add robust ELF EEmem discovery for Flatpak/Snap/PIE PCSX2 builds. ([`4c083a1`](https://github.com/luni/DarkCloud-Enhanced/commit/4c083a1fff27d5d46578d1f061206cf947253a79))
- Add Linux smoke test for PIE/Flatpak ELF EEmem discovery. ([`bcee5e3`](https://github.com/luni/DarkCloud-Enhanced/commit/bcee5e3f00ffb444c51b11774cfdd355983032df))
- Ignore generated binaries in linux smoke test directory. ([`6a963ce`](https://github.com/luni/DarkCloud-Enhanced/commit/6a963ce8410b042caead9e8fb84099e25bf3a82b))
- Add PAL verification scripts and address map to the repo. ([`f0c9f0a`](https://github.com/luni/DarkCloud-Enhanced/commit/f0c9f0a7333965786ee4b9fccdbdfa03fc723743))
- Improve PCSX2 process detection for Flatpak/Snap wrappers. ([`ce6a3bb`](https://github.com/luni/DarkCloud-Enhanced/commit/ce6a3bb0a16388a6013cf2f8b8769889219077f4))
- Update README with Linux, Flatpak, and verification instructions. ([`ffb9083`](https://github.com/luni/DarkCloud-Enhanced/commit/ffb9083cc650e3df075be4f3e6e1e6ab4c0d32f5))
- Add CI tests for the PAL port and Linux memory path. ([`232dd8c`](https://github.com/luni/DarkCloud-Enhanced/commit/232dd8c9e4c80b753d9036700e73191e86b27de3))

---

## Original v1.00 Release Notes

> The text below is the original markdown conversion of `Full_Change_Log_Public_Release_v1.00.pdf`.


> This markdown file is a conversion of the original `Full_Change_Log_Public_Release_v1.00.pdf`.

## Page 1

- Change Log Public Release v1.00 Town
- Allies can now be used in town areas and indoors;
- Town NPCs hold all new dialogues for each ally;
- Added new repeatable side quests to some NPCs;
- Added new unique side quests to some NPCs;
- NPC Pickle (Brownboo Village) can now track the player’s % completion;
- Time is now also tracked in Yellow Drops and Dark Heaven Castle;
- The clock was added to the Yellow Drops and Dark Heaven Castle areas;
- The town music was changed to start playing earlier in the day;
- Increased the radius from where a fish can pull down a bait; Shop
- Added a new daily cycle-based item slot in some shops;
- Reduced the selling price for all baits;
- Changed the prices for the Treasure Key and all the Amulet items;
- Changed the prices for all weapons;
- Removed the Endurance attachment from Gaffer’s shop;

## Page 2

- Dungeon
- Added the ability to swap weapon elements quickly via the directional pad on the controller (Up and
  Down);
- There is now a chance to spawn a more powerful version of an enemy when entering a dungeon floor;
- Added the ability to exit the dungeon during the floor selection screen;
- Adjusted the weapons loot tables to be more relevant for each dungeon;
- Adjusted the big chest spawns for each dungeon;
- Weapons can now also drop in back floors;
- Repair Powders and Escape Powders can now be used as active items;
- Repair Powders and Escape Powders and Revival Powders can now be stacked on the active item slots;
- Reduced the Mystery Circle – “Funds Increased” effect from 2.2 times to 1.5 times;
- Re-implemented an unused dungeon floor clear message that triggers whenever you fully defeat all the
  enemies in a floor; General
- Improved Ungaga’s item throwing animation speed;
- Reduced Ungaga’s Fog Gate cutscene duration;
- Added an input shortcut to soft reboot the game back to the “Title Screen” (Hold R1 + R2 + L1 + L2 +
  Select + Start for 3 seconds);
- Added an option to toggle some graphical improvements (Can be found in the mod window, under “Page
  2”);
- Added an option to toggle the weapon breaking beeping sound On/Off (Can be found in the mod window,
  under “Page 2”);
- Added an option to toggle the battle or all music On/Off (Can be found in the mod window, under
  “Page 2”);
- Added an option to toggle character attack voices On/Off (Can be found in the mod window, under
  “Page 2”);

## Page 3

- Added cheat codes to the game (Only work inside dungeons and while paused. Requires 10 inputs) Cheat
  Inputs Broken Dagger Triangle L1 R2 Cross Left Up L2 Circle Right Select Attach. God Down Up
  Square Circle Select Right Left Circle Square R1 Mode Power Up L2 Down Select Square Triangle R2
  Up Right L1 Cross Powders Max R2 Left L3 Cross Up Select R1 Triangle Square Down Money Unlock R3
  Triangle Up Select L2 R2 Left Select Circle R1 Floors Special Menus Select R3 Down Triangle Up
  Cross Select L3 R1 L1 Part 1 Special Menus Circle L1 Right Left R3 R1 Square Cross Select Cross
  Part 2
- For the dungeon debug menu, press and hold the R3 button during walk mode;
- For the inventory debug menu, press L3 while on the Bag menu; Fixes
- Fixed the Broken Dagger glitch, where you could retrieve a very powerful broken dagger attachment
  from shop menus;
- Fixed the Out of Bounds menu glitch, where you could go out of some menu’s boundaries under some
  circumstances;
- Fixed a bug where the item “Flapping Duster” could not spawn in Gallery of Time;
- Fixed a bug where the game would not actually save when prompted to do so after the credits
  sequence;

## Page 4

- Weapons
- Added new custom abilities to some weapons (Refer to the weapon balance list);
- Re-adjusted some weapons stats and pricing: Baselard - Increased Endurance 20 🠖 30; Antique Sword -
  Decreased Speed 50 🠖 30; - Increased Fire 6 🠖 15; Kitchen Knife - Increased WHp 30 🠖 50; -
  Increased Attack 5 🠖 25; - Reduced Ice 8 🠖 0; - Increased Thunder 0 🠖 8; - Increased Sea Killer 33
  🠖 90; - Removed build up options; Tsukikage - Increased Endurance 20 🠖 33; - Increased Speed 70 🠖
  80; Macho Sword - Added the ABS ability; Heaven’s Cloud - Increased attachment slot 2 🠖 3; - Added
  chance to get Poison or Critical abilities;

## Page 5

- Lamb’s Sword - Increased attachment slot 2 🠖 3
- - Decreased the required threshold to transform into Wolf Mode 20% 🠖 50%
- Dark Cloud - Added chance to get Poison or Stop abilities
- Brave Ark - Increased attachment slot 2 🠖 3
- Big Bang - Increased Speed 60 🠖 70
- - Added chance to get Critical or Stop abilities
- Atlamillia Sword - Added chance to get Heal or Stop abilities
- Mardan Eins - Added custom ability – One measly rod ain’t gonna cut it
- Mardan Twei - Added custom ability – Second time’s the charm? You’re not getting off the hook yet
- Arise Mardan - Added custom ability – Now you’re opening a can of worms. Arise Mardan!

## Page 6

- Small Sword - Increased WHp 30 🠖 35
- - Decreased Magic 36 🠖 17
- - Decreased Sea Killer 33 🠖 0
- - Increased Metal Breaker 0 🠖 10
- Sand Breaker - Increased WHp 40 🠖 45
- - Increased Endurance 20 🠖 25
- - Increased attachment slot 2 🠖 3
- Drain Seeker - Increased WHp 40 🠖 60
- Chopper - Increased Speed 50 🠖 60
- Choora - Increased WHp 30 🠖 57
- - Increased Attack 39 🠖 45
- - Decreased Speed 80 🠖 70
- - Increased Ice 8 🠖 10
- - Increased Thunder 0 🠖 15
- - Increased Undead Buster 0 🠖 15
- - Increased Beast Buster 0 🠖 15
- - Increased Metal Breaker 0 🠖 15
- - Increased attachment slot 2 🠖 3

## Page 7

- Claymore - Increased Undead Buster 0 🠖 10
- - Increased Beast Buster 8 🠖 10
- - Increased Mage Slayer 8 🠖 10
- Maneater - Increased Endurance 40 🠖 44
- - Increased Speed 50 🠖 70
- - Decreased Magic 55 🠖 45
- - Increased Ice 0 🠖 15
- - Increased Thunder 10 🠖 15
- - Increased Holy 0 🠖 15
- - Increased Undead Buster 10 🠖 15
- - Increased Beast Buster 10 🠖 15
- - Increased Metal Breaker 10 🠖 15
- - Increased Mimic Breaker 0 🠖 10
- Bone Rapier - Increased WHp 30 🠖 38
- - Decreased Magic 30 🠖 26
- - Added custom ability – Bones and rocks may break my locks
- Sax - Increased Speed 50 🠖 60
- - Increased Fire 0 🠖 6
- - Increased Sky Hunter 0 🠖 10

## Page 8

- 7 Branch Sword - Increased WHp 30 🠖 47
- - Increased Endurance 40 🠖 47
- - Increased Magic 35 🠖 37
- - Increased Dino Slayer 0 🠖 7
- - Increased Undead Buster 0 🠖 7
- - Increased Sea Killer 0 🠖 7
- - Increased Stone Breaker 0 🠖 7
- - Increased Plant Buster 0 🠖 7
- - Increased Beast Buster 0 🠖 8
- - Increased Sky Hunter 0 🠖 7
- - Increased Metal Breaker 0 🠖 10
- - Increased Mimic Breaker 0 🠖 7
- - Increased Mage Slayer 0 🠖 8
- Dusack - Added chance to get Steal ability
- Cross Hinder - Increased Endurance 40 🠖 50
- - Increased Speed 60 🠖 70
- - Increased Magic 21 🠖 32
- 7th Heaven - Added custom ability – Two materials of the same kind mean De Ja Vu
- Sword of Zeus - Added custom ability – Thunder is just another way to fuel potential

## Page 9

- Chronicle - Added custom ability – Enemies tremble before the echoes of your slashes
- Chronicle 2 - Increased Max Attack 350 🠖 999
- - Added custom ability – Treasures aplenty
- Wooden Slightshot - Increased Attack 4 🠖 6
- - Increased Magic 0 🠖 2
- - Increased Fire 0 🠖 4
- Bandit Slightshot - Removed build-up option to Hardshooter
- Bone Slightshot - Increased Attack 8 🠖 11
- - Increased Endurance 20 🠖 30
- Hardshooter - Decreased Speed 70 🠖 60
- Matador - Added Critical ability
- Angel Gear - Added custom ability – Angels also love humans not just cats
- Turtle Shell - Increased Magic 0 🠖 10

## Page 10

- Big Bucks Hammer - Removed the build-up option to Gaia Hammer
- Frozen Tuna - Decreased WHp 80 🠖 65
- Gaia Hammer - Increased Endurance 10 🠖 25
- Tall Hammer - Added custom ability – Try messing with someone your own size
- Trial Hammer - Increased Attack 20 🠖 30
- - Increased Endurance 10 🠖 25
- Inferno - Added custom ability – With great power comes great risk

## Page 11

- Gold Ring - Increased Attack 10 🠖 15
- - Increased Magic 20 🠖 30
- Bandit’s Ring - Increased Attack 12 🠖 30
- - Increased Max Attack 38 🠖 50
- - Decreased Magic 25 🠖 20
- - Changed build-up options to Crystal Ring and Thorn Armlet
- Platinum Ring - Increased Attack 17 🠖 30
- Goddess Ring - Added chance to get Heal ability
- Destruction Ring - Added chance to get Critical ability
- Satan’s Ring - Added chance to get Drain ability
- Athenas Armlet - Added ABS up ability
- Mobius Ring - Added custom ability – The longer it goes, the harder it hits

## Page 12

- Pocklekul - Decreased Attack 35 🠖 28
- - Decreased Magic 55 🠖 28
- - Decreased Holy 20 🠖 0
- - Changed build-up options to Fairy Ring and Thorn Armlet
- Thorn Armlet - Increased Max Magic 46 🠖 65
- - Changed build-up option to Destruction Ring
- Secret Armlet - Added custom ability – Great at keeping bad secrets in, not so much for the good
  ones
- Ungaga Weapons (Except Babel Spear) - Increased Attack +10
- - Increased Max Attack +10
- - Increased Endurance +15
- Hercules’ Wrath - Added custom ability – Strength is for those who know how to take a beating
- Babel Spear - Increased attachment slot 3 🠖 4
- - Added custom ability – Knowledge is nothing but a concept. Time is everlasting

## Page 13

- Osmond Weapons - Increased Attack +15
- - Increased Max Attack +15
- Skunk - Added chance to get Poison ability
- - Added the build-up option for Hex-a-Blaster
- Star Breaker - Added custom ability – Small shooting stars usually grant the biggest wishes
- Supernova - Added custom ability – Supernovas come in all shapes and colors
- Swallow - Added chance to get Steal ability
