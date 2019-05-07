module.exports = async function getProductDetail(req) {
    let items, size;
    let singleItem;
    const productId = +req.params.productId;
  
    const collection =
      process.env.COLLECTION_NAME ||
      (req.keyvault &&
        req.keyvault.secrets &&
        req.keyvault.secrets["COLLECTION-NAME"]) ||
      "inventory";
    try {      
      // [items, size] = await Promise.all([
      //   req.mongo.db
      //     .collection(collection)
      //     .find({id:productId})          
      //     .toArray(),
      //   req.mongo.db
      //     .collection(collection)
      //     .find({id:productId})
      //     .count()
      // ]);

      [singleItem, size] = await Promise.all([
        req.mongo.db
          .collection(collection)
          .findOne({id:productId}),
        req.mongo.db
          .collection(collection)
          .find({id:productId})
          .count()
      ]);

    } catch (e) {
      console.error("e", e);
    }
  
    return singleItem;
  };
  