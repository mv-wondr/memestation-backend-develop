// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
//The default owner of the contract is who deployed it.
// does memestation want to transferownership/renounce ?
//AccessControl for role-based access control, do memestation need this ?
contract MemeStationRinkeby is Ownable, ERC721Enumerable {
  //type to max instance
  mapping(string => uint256) private _maxTypeCount;
  //type to current instance
  mapping(string => uint256) private _currentTypeCount;
  mapping(uint256 => string) private _IPFSHashes;
  string private _baseUri;
  constructor(string memory baseURI) ERC721("Pepe", "Pepe NFT"){
    _baseUri = baseURI;
  }
  function createMemeType(string memory memeType, uint256 maxCount) public onlyOwner {
    require(_maxTypeCount[memeType] == 0, "This type has already been created.");
    _maxTypeCount[memeType] = maxCount;
  }
  function typeExist(string memory memeType) public view returns (bool) {
    return _maxTypeCount[memeType] > 0;
  }
  function isLimitReached(string memory memeType) public view returns (bool) {
    return _maxTypeCount[memeType] <= _currentTypeCount[memeType];
  }
  function mintMeme(address receiver, string memory memeType, string memory tokenIPFS) public onlyOwner {
    require(typeExist(memeType), "Type doesn't exist.");
    require(!isLimitReached(memeType), "Limit for this type is reached.");
    uint256 tokenId = totalSupply();
    _safeMint(receiver, tokenId);
    _currentTypeCount[memeType] += 1;
    _IPFSHashes[tokenId] = tokenIPFS;
  }
  function _baseURI() internal view override returns (string memory) {
    return _baseUri;
  }
  function tokenURI(uint256 tokenId) public view virtual override returns (string memory) {
    require(_exists(tokenId), "ERC721Metadata: URI query for nonexistent token");
    string memory baseURI = _baseURI();
    string memory ipfs_hash = _IPFSHashes[tokenId];
    return bytes(baseURI).length > 0 ? string(abi.encodePacked(baseURI, ipfs_hash)) : "";
  }
}
