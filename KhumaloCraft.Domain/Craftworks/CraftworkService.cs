using KhumaloCraft.Domain.Structs;

namespace KhumaloCraft.Domain.Craftworks;

public sealed class CraftworkService(
    ICraftworkRepository craftworkRepository) : ICraftworkService
{
    private static readonly List<Craftwork> _craftworks =
    [
        new Craftwork
        {
            Id = 1,
            Title = "Handcrafted Wooden Bowl",
            Description = "A beautifully handcrafted wooden bowl made from local wood.",
            Price = new Money(350.00m),
            ImageUrl = "/images/craftworks/craftwork_1.png",
            Category = CraftworkCategory.Woodworking
        },
        new Craftwork
        {
            Id = 2,
            Title = "Beaded Necklace",
            Description = "An elegant beaded necklace with intricate patterns.",
            Price = new Money(150.00m),
            ImageUrl = "/images/craftworks/craftwork_2.png",
            Category = CraftworkCategory.Beadwork
        },
        new Craftwork
        {
            Id = 3,
            Title = "Pottery Vase",
            Description = "A vibrant pottery vase hand-painted with traditional designs.",
            Price = new Money(548.98m),
            ImageUrl = "/images/craftworks/craftwork_3.png",
            Category = CraftworkCategory.Pottery
        },
        new Craftwork
        {
            Id = 4,
            Title = "Woven Basket",
            Description = "A sturdy woven basket perfect for carrying goods or as a decorative piece.",
            Price = new Money(200.00m),
            ImageUrl = "/images/craftworks/craftwork_4.png",
            Category = CraftworkCategory.Weaving
        },
        new Craftwork
        {
            Id = 5,
            Title = "Leather Wallet",
            Description = "A sleek leather wallet handcrafted for durability and style.",
            Price = new Money(400.00m),
            ImageUrl = "/images/craftworks/craftwork_5.png",
            Category = CraftworkCategory.Leatherworking
        },
        new Craftwork
        {
            Id = 6,
            Title = "Handmade Candle",
            Description = "A set of hand-poured candles with calming lavender scent.",
            Price = new Money(120.00m),
            ImageUrl = "/images/craftworks/craftwork_6.png",
            Category = CraftworkCategory.MixedMedia
        },
        new Craftwork
        {
            Id = 7,
            Title = "Ceramic Mug",
            Description = "A set of ceramic mugs featuring unique hand-painted designs.",
            Price = new Money(180.00m),
            ImageUrl = "/images/craftworks/craftwork_7.png",
            Category = CraftworkCategory.Pottery
        },
        new Craftwork
        {
            Id = 8,
            Title = "Artisan Quilt",
            Description = "A quilt made with traditional techniques and vibrant fabrics.",
            Price = new Money(650.00m),
            ImageUrl = "/images/craftworks/craftwork_8.png",
            Category = CraftworkCategory.TraditionalCrafts
        },
        new Craftwork
        {
            Id = 9,
            Title = "Hand-carved Statue",
            Description = "A detailed hand-carved statue depicting traditional figures.",
            Price = new Money(800.00m),
            ImageUrl = "/images/craftworks/craftwork_9.png",
            Category = CraftworkCategory.Sculpture
        },
        new Craftwork
        {
            Id = 10,
            Title = "Embroidered Pillow",
            Description = "A pillow with intricate embroidery showcasing local craftsmanship.",
            Price = new Money(250.00m),
            ImageUrl = "/images/craftworks/craftwork_10.png",
            Category = CraftworkCategory.Sewing
        },
        new Craftwork
        {
            Id = 11,
            Title = "Beaded Earrings",
            Description = "Handmade beaded earrings with colorful patterns.",
            Price = new Money(100.00m),
            ImageUrl = "/images/craftworks/craftwork_11.png",
            Category = CraftworkCategory.Beadwork
        },
        new Craftwork
        {
            Id = 12,
            Title = "Wooden Clock",
            Description = "A stylish wooden clock with a classic design.",
            Price = new Money(450.00m),
            ImageUrl = "/images/craftworks/craftwork_12.png",
            Category = CraftworkCategory.Woodworking
        },
        new Craftwork
        {
            Id = 13,
            Title = "Handmade Soap",
            Description = "Natural handmade soap with soothing ingredients.",
            Price = new Money(80.00m),
            ImageUrl = "/images/craftworks/craftwork_13.png",
            Category = CraftworkCategory.RecycledCrafts
        },
        new Craftwork
        {
            Id = 14,
            Title = "Leather Belt",
            Description = "A rugged leather belt crafted for strength and style.",
            Price = new Money(350.00m),
            ImageUrl = "/images/craftworks/craftwork_14.png",
            Category = CraftworkCategory.Leatherworking
        },
        new Craftwork
        {
            Id = 15,
            Title = "Handwoven Rug",
            Description = "A handwoven rug featuring traditional patterns and colors.",
            Price = new Money(900.00m),
            ImageUrl = "/images/craftworks/craftwork_15.png",
            Category = CraftworkCategory.Textiles
        },
        new Craftwork
        {
            Id = 16,
            Title = "Ceramic Pot",
            Description = "A handcrafted ceramic pot with a modern design.",
            Price = new Money(300.00m),
            ImageUrl = "/images/craftworks/craftwork_16.png",
            Category = CraftworkCategory.Pottery
        },
        new Craftwork
        {
            Id = 17,
            Title = "Wooden Jewelry Box",
            Description = "An elegant wooden jewelry box with intricate carvings.",
            Price = new Money(600.00m),
            ImageUrl = "/images/craftworks/craftwork_17.png",
            Category = CraftworkCategory.Woodworking
        },
        new Craftwork
        {
            Id = 18,
            Title = "Artisan Basket",
            Description = "A beautifully crafted basket made from natural fibers.",
            Price = new Money(220.00m),
            ImageUrl = "/images/craftworks/craftwork_18.png",
            Category = CraftworkCategory.Textiles
        },
        new Craftwork
        {
            Id = 19,
            Title = "Hand-painted Plate",
            Description = "A decorative plate hand-painted with unique designs.",
            Price = new Money(160.00m),
            ImageUrl = "/images/craftworks/craftwork_19.png",
            Category = CraftworkCategory.Pottery
        },
        new Craftwork
        {
            Id = 20,
            Title = "Leather Handbag",
            Description = "A stylish leather handbag designed for everyday use.",
            Price = new Money(750.00m),
            ImageUrl = "/images/craftworks/craftwork_20.png",
            Category = CraftworkCategory.Leatherworking
        },
        new Craftwork
        {
            Id = 21,
            Title = "Beaded Bracelet",
            Description = "A vibrant beaded bracelet handcrafted with care.",
            Price = new Money(130.00m),
            ImageUrl = "/images/craftworks/craftwork_21.png",
            Category = CraftworkCategory.Beadwork
        },
        new Craftwork
        {
            Id = 22,
            Title = "Handcrafted Wooden Chair",
            Description = "A comfortable and stylish wooden chair made by skilled artisans.",
            Price = new Money(1200.00m),
            ImageUrl = "/images/craftworks/craftwork_22.png",
            Category = CraftworkCategory.Woodworking
        },
        new Craftwork
        {
            Id = 23,
            Title = "Ceramic Planter",
            Description = "A beautifully designed ceramic planter for your plants.",
            Price = new Money(200.00m),
            ImageUrl = "/images/craftworks/craftwork_23.png",
            Category = CraftworkCategory.Pottery
        },
        new Craftwork
        {
            Id = 24,
            Title = "Embroidered Tablecloth",
            Description = "A tablecloth with exquisite embroidery that adds elegance to any dining setting.",
            Price = new Money(350.00m),
            ImageUrl = "/images/craftworks/craftwork_24.png",
            Category = CraftworkCategory.Sewing
        },
        new Craftwork
        {
            Id = 25,
            Title = "Handmade Wooden Toy",
            Description = "A classic wooden toy hand-carved for children.",
            Price = new Money(150.00m),
            ImageUrl = "/images/craftworks/craftwork_25.png",
            Category = CraftworkCategory.Woodworking
        }
    ];

    public IEnumerable<Craftwork> GetAllCraftworks()
    {
        return _craftworks;
    }

    public Craftwork GetCraftworkById(int id)
    {
        return _craftworks.Single(c => c.Id == id);
    }

    public bool TryGetCraftworkById(int id, out Craftwork craftwork)
    {
        craftwork = _craftworks.SingleOrDefault(c => c.Id == id);

        return craftwork != null;
    }

    public IEnumerable<Craftwork> FetchAllCraftworks()
    {
        return craftworkRepository.Query().ToList();
    }

    public Craftwork FetchByCraftworkId(int id)
    {
        return craftworkRepository.Query().Single(c => c.Id == id);
    }

    public bool TryFetchByCraftworkId(int id, out Craftwork craftwork)
    {
        craftwork = craftworkRepository.Query().SingleOrDefault(c => c.Id == id);

        return craftwork != null;
    }

    public void AddCraftwork(Craftwork craftwork)
    {
        //TODO-LP : Add business rules / validations
        craftworkRepository.Upsert(craftwork);
    }

    public void UpdateCraftwork(Craftwork craftwork)
    {
        //TODO-LP : Add business rules / validations
        craftworkRepository.Upsert(craftwork);
    }
}