using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using System.Linq;


public class StoreManager : MonoBehaviour
{
    public GameObject Plywood;
    public GameObject Floor;
    public GameObject Walls;
    public GameObject PriceTag;
    public GameObject LeftHingeDoor;
    public GameObject RightHingeDoor;
    public GameObject SlidingDoor;
    public ExpirationDate expiration;
    public float StoreLength;
    public float StoreWidth;
    public List<(float Length, float Width, float XCenter, float ZCenter)> subdivisions = new List<(float, float, float, float)>();
    public Dictionary<string, float> priceDictionary = new Dictionary<string, float>();
    
    public Dictionary<string, GameObject> shelves = new Dictionary<string, GameObject>();
    public Dictionary<string, List<List<GameObject>>> Categories = new Dictionary<string, List<List<GameObject>>>();
    
    public HashSet<string> taggedProducts = new HashSet<string>(); // Track products that already have price tags
    
    public GroceryData groceryData;
    public int Seed;
    public string StoreLayout;

    public enum PriceUnit
    {
        Peso,
        Dollar
    }

    public PriceUnit selectedPriceUnit = PriceUnit.Peso; // Default to Peso
    // Start is called before the first frame update
    void Start()
    {
        StoreLayout = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        LoadJson();
        PrintCategories();
        LoadProducts();
        LoadShelves();
        LoadPricesFromCSV();
        Seed = Random.Range(0, 100); // Generate a random seed
        FillShelves(Seed, 270);
        Debug.Log("StoreManager done.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetEnvironment();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchEnvironment("Store1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchEnvironment("Store2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchEnvironment("Store3");
        }
    }

    [System.Serializable]
    public class GroceryCategory
    {
        public string Category;
        public List<string> Items;
    }
    [System.Serializable]
    public class GroceryData
    {
        public List<GroceryCategory> Categories;
    }

    void LoadJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Categories");
        if (jsonFile != null)
        {
            string json = jsonFile.text;
            groceryData = JsonUtility.FromJson<GroceryData>(json);
        }
        else
        {
            Debug.LogError("Categories.json file not found in Resources folder!");
        }
    }

    void LoadShelves()
    {
        Shelf[] allShelves = FindObjectsByType<Shelf>(FindObjectsSortMode.None);
        foreach (Shelf shelf in allShelves)
        {
            if (!shelves.ContainsKey(shelf.id))
            {
            shelves.Add(shelf.id, shelf.gameObject);
            Debug.Log("Shelf ID: " + shelf.id + ", Length: " + shelf.length + ", Width: " + shelf.width + ", Height: " + shelf.height + ", Thickness: " + shelf.thickness + ", Level: " + shelf.level + ", Categories for shelf " + shelf.id + ": " + string.Join(", ", shelf.Categories));
            }
            else
            {
                Debug.LogWarning("Duplicate Shelf ID found: " + shelf.id);
            }
        }
    }
    void PrintCategories()
    {
        foreach (GroceryCategory category in groceryData.Categories)
        {
            Debug.Log("Category: " + category.Category);
            foreach (string item in category.Items)
            {
                Debug.Log("Item: " + item);
            }
        }
    }

    void LoadProducts()
    {
        // Load all products from the Resources folder
        foreach (var category in groceryData.Categories)
        {
            float totalLength = 0f;
            float totalWidth = 0f;
            float totalHeight = 0f;
            int productCount = 0;

            foreach (var item in category.Items)
            {
            GameObject product = Resources.Load<GameObject>("Prefabs/Products/" + item);
            if (product != null)
            {
                if (!Categories.ContainsKey(category.Category))
                {
                Categories[category.Category] = new List<List<GameObject>>();
                }
                Categories[category.Category].Add(new List<GameObject> { product });

                // Add XRGrabInteractable if not present
                if (product.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() == null)
                {
                product.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                }
                ProductInfo productInfoComponents = product.GetComponentInChildren<ProductInfo>();

                if (product.GetComponent<ProductInfo>() == null)
                {
                product.AddComponent<ProductInfo>();
                }
                product.GetComponent<ProductInfo>().ProductName = item;
                product.GetComponent<ProductInfo>().Category = category.Category;
                UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = product.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking;
                grabInteractable.useDynamicAttach = true;

                // Calculate average length, width, and height
                MeshRenderer renderer = product.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                renderer = product.GetComponentInChildren<MeshRenderer>();
                }
                if (renderer != null)
                {
                totalLength += renderer.bounds.size.x;
                totalWidth += renderer.bounds.size.z;
                totalHeight += renderer.bounds.size.y;
                productCount++;
                }
            }
            else
            {
                Debug.LogError("Product not found: " + item);
            }
            }

            if (productCount > 0)
            {
            float avgLength = totalLength / productCount;
            float avgWidth = totalWidth / productCount;
            float avgHeight = totalHeight / productCount;
            Debug.Log($"Category: {category.Category} - Average Length: {avgLength:F3}, Average Width: {avgWidth:F3}, Average Height: {avgHeight:F3}");
            }
            else
            {
            Debug.LogWarning($"Category: {category.Category} has no valid products for average calculation.");
            }
        }
    }

    private IEnumerator FreeUpMemory()
    {
        // Wait for the end of the frame to ensure all objects are instantiated
        yield return new WaitForEndOfFrame();

        // Unload unused assets to free up memory
        Resources.UnloadUnusedAssets();
        Debug.Log("Unused assets have been unloaded to free up memory.");
    }

    public void FillShelves(int seed, float rotation)
    {
        int pseudoseed = 0;
        foreach (var shelfEntry in shelves)
        {
            Debug.Log("Filling shelf: " + shelfEntry.Key);
            string shelfId = shelfEntry.Key;
            Shelf shelfComponent = shelfEntry.Value.GetComponent<Shelf>();
            shelfComponent.GetShelfCategories(shelfComponent);
            Debug.Log("Fill " + shelfComponent.id + " with: " + string.Join(", ", shelfComponent.Categories));
            SpawnProducts(seed + pseudoseed, shelfId, rotation, shelfComponent.Categories, true);
            pseudoseed++;
        }

        // Free up unused texture memory
        StartCoroutine(FreeUpMemory());
    }

    public void SpawnShelf(string ShelfId, float x, float z, float length, float width, float height, float thickness, int level, float rotation, string DoorType = "None", bool SavePrefab = false)
    {
        List<GameObject> Panels = new List<GameObject>();

        for(int i=0; i<level; i++)
        {
            Vector3 BottomPanelPosition = new Vector3(0, thickness/2+(height+thickness)*i, 0);
            Vector3 BottomPanelScale = new Vector3(length, thickness, width);
            GameObject BottomPanel = Instantiate(Plywood, BottomPanelPosition, Quaternion.identity);
            BottomPanel.transform.localScale = BottomPanelScale;
            Panels.Add(BottomPanel);
        }
        
        if (DoorType != "None")
        {
            Vector3 LeftPanelPosition = new Vector3(-1*length/2-thickness/2, ((height+thickness)*level+thickness)/2, thickness/2);
            Vector3 LeftPanelScale = new Vector3(thickness, (height+thickness)*level+thickness, width+thickness);
            GameObject LeftPanel = Instantiate(Plywood, LeftPanelPosition, Quaternion.identity);
            LeftPanel.transform.localScale = LeftPanelScale;
            Panels.Add(LeftPanel);

            Vector3 RightPanelPosition = new Vector3(length/2+thickness/2, ((height+thickness)*level+thickness)/2, thickness/2);
            Vector3 RightPanelScale = new Vector3(thickness, (height+thickness)*level+thickness, width+thickness);
            GameObject RightPanel = Instantiate(Plywood, RightPanelPosition, Quaternion.identity);
            RightPanel.transform.localScale = RightPanelScale;
            Panels.Add(RightPanel);

            Vector3 TopPanelPosition = new Vector3(0, thickness/2+(height+thickness)*level, thickness / 2);
            Vector3 TopPanelScale = new Vector3(length, thickness, width + thickness);
            GameObject TopPanel = Instantiate(Plywood, TopPanelPosition, Quaternion.identity);
            TopPanel.transform.localScale = TopPanelScale;
            Panels.Add(TopPanel);
        }

        Vector3 BackPanelPosition = new Vector3(0, ((height+thickness)*level+thickness)/2, width/2 + thickness/2);
        Vector3 BackPanelScale = new Vector3(length, (height+thickness)*level+thickness, thickness);
        GameObject BackPanel = Instantiate(Plywood, BackPanelPosition, Quaternion.identity);
        BackPanel.transform.localScale = BackPanelScale;
        Panels.Add(BackPanel);
        

        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (GameObject obj in Panels)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = meshFilter.sharedMesh;
                ci.transform = obj.transform.localToWorldMatrix;
                combine.Add(ci);
                obj.SetActive(false);
                Destroy(obj);
            }
        }
        GameObject NewShelf = new GameObject(ShelfId);
        MeshFilter ShelfMeshFilter = NewShelf.AddComponent<MeshFilter>();
        MeshRenderer ShelfMeshRenderer = NewShelf.AddComponent<MeshRenderer>();

        Shelf TaggedShelf = NewShelf.AddComponent<Shelf>();
        TaggedShelf.id = ShelfId;
        TaggedShelf.length = length;
        TaggedShelf.width = width;
        TaggedShelf.height = height;
        TaggedShelf.thickness = thickness;
        TaggedShelf.level = level;
        TaggedShelf.rotation = rotation;

        shelves[ShelfId] = NewShelf;

        NewShelf.transform.position = new Vector3(x, 0, z);
        NewShelf.transform.rotation = Quaternion.Euler(0, rotation, 0);
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine.ToArray(), true, true);
        ShelfMeshFilter.mesh = combinedMesh;

        MeshCollider ShelfMeshCollider = NewShelf.AddComponent<MeshCollider>();
        ShelfMeshCollider.sharedMesh = combinedMesh;

        ShelfMeshRenderer.materials = Panels[0].GetComponent<MeshRenderer>().materials;

        if (DoorType == "LeftHingeDoor")
            {
                Vector3 DoorPosition = NewShelf.transform.position + NewShelf.transform.rotation * new Vector3(0, (thickness/2+(height+thickness)*level)/2, -(width + 2*thickness) / 2);
                GameObject Door = Instantiate(LeftHingeDoor, DoorPosition, NewShelf.transform.rotation);
                Door.transform.localScale = new Vector3(length, thickness/2+(height+thickness)*level, 1);
                Door.transform.SetParent(shelves[ShelfId].transform);

                Rigidbody rb = Door.AddComponent<Rigidbody>();
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = Door.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking;

                HingeJoint hingeJoint = Door.AddComponent<HingeJoint>();
                hingeJoint.anchor = new Vector3(-0.5f, 0, 0);
                hingeJoint.axis = new Vector3(0, 1, 0);
                hingeJoint.limits = new JointLimits { min = 0, max = 120 };
                hingeJoint.useLimits = true;
            }

        if (DoorType == "RightHingeDoor")
            {
                Vector3 DoorPosition = NewShelf.transform.position + NewShelf.transform.rotation * new Vector3(0, (thickness/2+(height+thickness)*level)/2, -(width + 2*thickness) / 2);
                GameObject Door = Instantiate(RightHingeDoor, DoorPosition, NewShelf.transform.rotation);
                Door.transform.localScale = new Vector3(length, thickness/2+(height+thickness)*level, 1);
                Door.transform.SetParent(shelves[ShelfId].transform);

                Rigidbody rb = Door.AddComponent<Rigidbody>();
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = Door.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.VelocityTracking;

                HingeJoint hingeJoint = Door.AddComponent<HingeJoint>();
                hingeJoint.anchor = new Vector3(0.5f, 0, 0);
                hingeJoint.axis = new Vector3(0, 1, 0);
                hingeJoint.limits = new JointLimits { min = -120, max = 0 };
                hingeJoint.useLimits = true;
            }
            
        else if (DoorType == "SlidingDoor")
        {
            Vector3 DoorPosition = NewShelf.transform.position + NewShelf.transform.rotation * new Vector3(0, (thickness/2+(height+thickness)*level)/2, -(width + 5*thickness) / 2);
            GameObject Door = Instantiate(SlidingDoor, DoorPosition, NewShelf.transform.rotation);
            Door.transform.localScale = new Vector3(length, thickness/2+(height+thickness)*level, 1);
            Door.transform.SetParent(shelves[ShelfId].transform);
        }
    }

    public void SpawnShelfGroup(string ShelfId, float CenterX, float CenterY, float ShelfWidth, float ShelfHeight, float ShelfThickness, int ShelfLevel, float ShelfLength, float Rotation, bool SavePrefab = false){
        float ShelfLengthSide = (ShelfWidth+ShelfThickness)*2;

        SpawnShelf(ShelfId+"_a", 0, - 0.5f*(ShelfWidth + ShelfThickness), ShelfLength, ShelfWidth, ShelfHeight, ShelfThickness, ShelfLevel, 0, "None", SavePrefab);
        SpawnShelf(ShelfId+"_b", 0, 0.5f*(ShelfWidth + ShelfThickness), ShelfLength, ShelfWidth, ShelfHeight, ShelfThickness, ShelfLevel, 180, "None", SavePrefab);
        SpawnShelf(ShelfId+"_c", -(ShelfThickness+ShelfLength+ShelfWidth)/2, 0, ShelfLengthSide, ShelfWidth, ShelfHeight, ShelfThickness, ShelfLevel, 90, "None", SavePrefab);
        SpawnShelf(ShelfId+"_d", (ShelfThickness+ShelfLength+ShelfWidth)/2, 0, ShelfLengthSide, ShelfWidth, ShelfHeight, ShelfThickness, ShelfLevel, 270, "None", SavePrefab);
        
        GameObject shelfGroup = new GameObject(ShelfId + "_Group");
        foreach (var shelf in new[] { ShelfId + "_a", ShelfId + "_b", ShelfId + "_c", ShelfId + "_d" })
        {
            shelves[shelf].transform.SetParent(shelfGroup.transform);
        }

        shelfGroup.transform.position = new Vector3(CenterX, 0, CenterY);
        shelfGroup.transform.rotation = Quaternion.Euler(0, Rotation, 0);
        
    }

    public void SpawnProducts(int seed, string ShelfId, float ProductOrientation, List<string> Categ, bool random = false, bool isBottomCenterPivot = false)
    {
        // Verify and print all valid categories
        foreach (string category in Categ)
        {
            Debug.Log(ShelfId + " Category: " + category);
            if (!this.Categories.ContainsKey(category))
            {
                Debug.LogError("Invalid category: " + category + ". Cancelling action.");
                return;
            }
        }

        Random.InitState(seed);
        GameObject shelf = shelves[ShelfId];
        Shelf shelfComponent = shelf.GetComponent<Shelf>();

        float length = shelfComponent.length;
        float width = shelfComponent.width;
        float height = shelfComponent.height;
        float thickness = shelfComponent.thickness;
        int level = shelfComponent.level;

        // Precompute partition starting points
        int partitions = Categ.Count;
        float partitionLength = length / partitions;
        List<Vector3> partitionStartPoints = new List<Vector3>();

        for (int i = 0; i < partitions; i++)
        {
            float startX = -length / 2 + (i * partitionLength);
            partitionStartPoints.Add(new Vector3(startX, 0, 0));
        }

        // Spawn products for each level
        for (int i = 0; i < level; i++)
        {
            // Adjust vertical position for bottom-centered pivot
            float yPosition = (i * (height + thickness)) + (thickness / 2);

            // Spawn the first row of products
            List<GameObject> firstRow = SpawnFirstRow(Seed + i, shelfComponent, partitionStartPoints, Categ, ProductOrientation, yPosition, partitionLength, isBottomCenterPivot);

            // Fill the depth of the shelf
            FillShelfDepth(shelfComponent, firstRow, yPosition, width, isBottomCenterPivot);

        }
        // Spawn price tags
        foreach (var category in Categ)
        {
            int currentLevel = 0; // Track the current level
            float cumulativeWidth = 0f; // Track the cumulative width of price tags
            foreach (var sublist in Categories[category])
            {
                foreach (var product in sublist)
                {
                    string productName = product.name;
                    float price = priceDictionary[productName];
                    string unit = "pc";

                    // Calculate the X position within the partition
                    float partitionOffset = Categ.IndexOf(category) * length / Categ.Count;

                    // Get the width of the price tag
                    float priceTagWidth = 0.071f; // Example width of the price tag (adjust as needed)

                    // Check if adding the current price tag exceeds the partition length
                    if (cumulativeWidth + priceTagWidth > length / Categ.Count)
                    {
                        // Move to the next level
                        cumulativeWidth = 0f; // Reset the cumulative width for the new level
                        currentLevel++; // Move to the next level
                    }

                    // Calculate the X position, ensuring the entire price tag fits within the partition
                    float centerX = -length / 2 + partitionOffset + cumulativeWidth + priceTagWidth / 2;

                    // Calculate the Y position (level)
                    float centerY = (thickness + height) * (level - 1 - currentLevel - 0.02f) + thickness / 2;

                    float centerZ = -width / 2;

                    // Check if the product is instantiated
                    if (GameObject.Find(productName) != null)
                    {
                        SpawnPriceTag(ShelfId, productName, price, unit, centerX, centerY, centerZ);
                    }

                    cumulativeWidth += priceTagWidth; // Increment the cumulative width
                }
            }
        }   
    }

    private List<GameObject> SpawnFirstRow(int Seed, Shelf shelfComponent, List<Vector3> partitionStartPoints, List<string> Categ, float ProductOrientation, float yPosition, float partitionLength, bool isBottomCenterPivot)
    {
        Random.InitState(Seed);
        List<GameObject> firstRow = new List<GameObject>();
        float firstItemOffset = 0.02f;

        // Spawn price tags for the products in the first row
            float spaceTakenForTags = 0; // Track space taken for price tags
            int currentLevel = 0; // Track the current level for price tags
        float length = shelfComponent.length;
        float width = shelfComponent.width;
        float height = shelfComponent.height;
        float thickness = shelfComponent.thickness;
        int level = shelfComponent.level;

        foreach (var (category, startPoint) in Categ.Zip(partitionStartPoints, (cat, pos) => (cat, pos)))
        {
            List<GameObject> products = new List<GameObject>();
            foreach (var sublist in Categories[category])
            {
                products.AddRange(sublist);
                Debug.Log("Shelf " + shelfComponent.id + ", Category: " + category + ", Products: " + string.Join(", ", sublist.Select(p => p.name)));
            }

            float spaceTaken = 0.03f;
            while (spaceTaken - 0.03f < partitionLength)
            {
                if (products.Count == 0)
                {
                    Debug.LogError($"No products available for category: {category}");
                    break;
                }

                GameObject item = products[Random.Range(0, products.Count)];
                MeshRenderer itemRenderer = item.GetComponent<MeshRenderer>();

                
                if (itemRenderer == null)
                {
                    Debug.Log($"Product {item.name} does not have a MeshRenderer. Must be the child.");
                    itemRenderer = item.GetComponentInChildren<MeshRenderer>();
                    Debug.Log("Child: " + itemRenderer);
                }

                float itemLength = ProductOrientation == 90 || ProductOrientation == 270
                    ? itemRenderer.bounds.size.z
                    : itemRenderer.bounds.size.x;

                float itemDepth = ProductOrientation == 90 || ProductOrientation == 270
                    ? itemRenderer.bounds.size.x + firstItemOffset // Add a small buffer for depth
                    : itemRenderer.bounds.size.z + firstItemOffset; // Add a small buffer for depth

                float itemHeight = itemRenderer.bounds.size.y;

                if (spaceTaken + itemLength > partitionLength)
                {
                    break;
                }

                // Calculate pivot offset
                Vector3 pivotOffset = isBottomCenterPivot
                    ? new Vector3(0, itemHeight / 2, 0) // Adjust for bottom-center pivot
                    : Vector3.zero; // No adjustment for center pivot

                // Calculate position for the item
                Vector3 position = shelfComponent.transform.position
                    + shelfComponent.transform.right * (startPoint.x + spaceTaken + itemLength / 2) // Adjust along the shelf's local right direction
                    + shelfComponent.transform.up * (yPosition) // Adjust vertically to place on top of the level
                    + shelfComponent.transform.forward * (-shelfComponent.width / 2 + itemDepth / 2); // Adjust to align with the back of the shelf

                position -= pivotOffset; // Apply pivot offset

                // Correct rotation for the product
                Quaternion rotation = shelfComponent.transform.rotation * Quaternion.Euler(0, ProductOrientation, 0);

                // Instantiate the item
                GameObject instantiatedItem = Instantiate(item, position, rotation);
                instantiatedItem.name = item.name;
                instantiatedItem.tag = "Grippable";
                instantiatedItem.transform.SetParent(shelfComponent.transform);
                instantiatedItem.GetComponent<ProductInfo>().Shelf = shelfComponent.id;


                // Turn off barcode and expiration date decals
                ProductInteractionHandler interactionHandler = instantiatedItem.AddComponent<ProductInteractionHandler>();
                Transform barcodePlane = instantiatedItem.transform.Find("Barcode");
                DecalProjector expirationDateDecal = instantiatedItem.transform.Find("Expiration Date")?.GetComponent<DecalProjector>();
                if (expirationDateDecal == null)
                {
                    expirationDateDecal = instantiatedItem.GetComponentInChildren<DecalProjector>();
                }
                string randomDate1 = System.DateTime.Now.AddDays(Random.Range(1, 365)).ToString("MM/dd/yyyy");
                string randomDate2 = System.DateTime.Now.AddDays(Random.Range(-365, 0)).ToString("MM/dd/yyyy");
                string randomDate = (Random.value < 0.1f) ? randomDate1 : randomDate2;
                
                expiration.SetExpirationDate(expirationDateDecal, "Best Before\n" + randomDate);
                

                if (barcodePlane != null)
                {
                    interactionHandler.BarcodePlane = barcodePlane.gameObject;
                    barcodePlane.gameObject.SetActive(false);
                }
                if (expirationDateDecal != null)
                {
                    interactionHandler.ExpirationDateDecal = expirationDateDecal.gameObject;
                    expirationDateDecal.gameObject.SetActive(false);
                }
                Debug.Log(shelfComponent.id + " category is: " + category);
                
                float offset = 0.01f;
                // Add stacks as long as the cumulative height is less than the height of the shelf
                float cumulativeHeight = itemHeight+0.02f; // Start with the height of the first item
                if (category == "Can" || category == "Soup" || category == "Noodles")
                {
                    while (cumulativeHeight + itemHeight <= 0.27)
                    {
                        Vector3 stackPosition = position + shelfComponent.transform.up * cumulativeHeight; // Adjust height for the stack
                        GameObject stackItem = Instantiate(item, stackPosition, rotation);
                        stackItem.name = item.name;
                        stackItem.tag = "Grippable";
                        stackItem.transform.SetParent(shelfComponent.transform);

                        // Turn off barcode and expiration date decals for the stack
                        ProductInteractionHandler stackInteractionHandler = stackItem.AddComponent<ProductInteractionHandler>();
                        Transform stackBarcodePlane = stackItem.transform.Find("Barcode");
                        DecalProjector stackExpirationDateDecal = stackItem.transform.Find("Expiration Date")?.GetComponent<DecalProjector>();
                        if (stackExpirationDateDecal == null)
                        {
                            stackExpirationDateDecal = stackItem.GetComponentInChildren<DecalProjector>();
                        }
                        string stackRandomDate = System.DateTime.Now.AddDays(Random.Range(1, 365)).ToString("MM/dd/yyyy");
                        
                        expiration.SetExpirationDate(stackExpirationDateDecal, "Best Before\n" + stackRandomDate);
                

                        if (stackBarcodePlane != null)
                        {
                            stackInteractionHandler.BarcodePlane = stackBarcodePlane.gameObject;
                            stackBarcodePlane.gameObject.SetActive(false);
                        }
                        if (stackExpirationDateDecal != null)
                        {
                            stackInteractionHandler.ExpirationDateDecal = stackExpirationDateDecal.gameObject;
                            stackExpirationDateDecal.gameObject.SetActive(false);
                        }

                        cumulativeHeight += itemHeight + 0.002f; // Increment cumulative height with a small buffer
                        Debug.Log($"Placed stack of {item.name} at position: {stackPosition}");
                    }

                    offset = 0.008f; // Reset offset for the next item
                }

                if (category == "Soda" || category == "Water" || category == "Juice" || category == "Noodles" || category == "Dairies")
                {
                    offset = 0.002f;
                }

                firstRow.Add(instantiatedItem);

                Debug.Log($"Placed {item.name} at position: {position} with length {itemLength}");
                spaceTaken += itemLength + offset; // Add buffer space
            }
        }

        return firstRow;
    }

    private void FillShelfDepth(Shelf shelfComponent, List<GameObject> firstRow, float yPosition, float shelfWidth, bool isBottomCenterPivot, bool stack = false)
    {
        // Use the shelf's local forward direction to calculate depth placement
        Vector3 depthDirection = shelfComponent.transform.forward;
        float depthOffset = 0.012f;


        foreach (GameObject firstRowItem in firstRow)
        {
            MeshRenderer itemRenderer = firstRowItem.GetComponent<MeshRenderer>();

            if (itemRenderer == null)
            {
                Debug.Log($"Product {firstRowItem.name} does not have a MeshRenderer. Must be the child.");
                itemRenderer = firstRowItem.GetComponentInChildren<MeshRenderer>();
                Debug.Log("Child: " + itemRenderer);
            }

            // Dynamically calculate the orientation of the first row product
            float productOrientation = firstRowItem.transform.eulerAngles.y;

            // Correctly calculate the depth of the product along its local x-axis
            float itemDepth = (productOrientation == 90 || productOrientation == 270)
                ? itemRenderer.bounds.size.z // Depth is along the z-axis when rotated 90° or 270°
                : itemRenderer.bounds.size.x; // Depth is along the x-axis for 0°, 180°, or -180°
            float itemHeight = itemRenderer.bounds.size.y;
            float remainingDepth = shelfWidth - itemDepth;

            // Start placing rows behind the first row
            Vector3 currentPosition = firstRowItem.transform.position;

            int limit_products = 10; // Limit the number of products to place in depth
            while (remainingDepth - itemDepth - 0.03 > itemDepth)
            {
                // if(limit_products <= 0)
                // {
                //     Debug.Log("Limit reached for depth placement.");
                //     break;
                // }
                // limit_products--;
                // Calculate pivot offset
                Vector3 pivotOffset = isBottomCenterPivot
                    ? new Vector3(0, itemRenderer.bounds.size.y / 2, 0) // Adjust for bottom-center pivot
                    : Vector3.zero; // No adjustment for center pivot

                // Calculate position for the next row using the shelf's local forward direction
                currentPosition += depthDirection * (itemDepth + depthOffset); // Move backward along the shelf's depth
                Vector3 position = currentPosition - pivotOffset; // Apply pivot offset

                // Correct rotation for the product
                Quaternion rotation = firstRowItem.transform.rotation;

                // Instantiate the item
                GameObject instantiatedItem = Instantiate(firstRowItem, position, rotation);
                instantiatedItem.name = firstRowItem.name;
                instantiatedItem.tag = "Grippable";
                instantiatedItem.transform.SetParent(shelfComponent.transform);

                // Turn off barcode and expiration date decals
                ProductInteractionHandler interactionHandler = instantiatedItem.AddComponent<ProductInteractionHandler>();
                Transform barcodePlane = instantiatedItem.transform.Find("Barcode");
                Transform expirationDateDecal = instantiatedItem.transform.Find("Expiration Date");

                if (barcodePlane != null)
                {
                    interactionHandler.BarcodePlane = barcodePlane.gameObject;
                    barcodePlane.gameObject.SetActive(false);
                }
                if (expirationDateDecal != null)
                {
                    interactionHandler.ExpirationDateDecal = expirationDateDecal.gameObject;
                    expirationDateDecal.gameObject.SetActive(false);
                }

                Debug.Log($"Placed {instantiatedItem.name} at position: {position} to fill depth");

                // Update remaining depth and move to the next row
                remainingDepth -= itemDepth + depthOffset; // Add buffer space
                // Add stacks as long as the cumulative height is less than the height of the shelf
                float cumulativeHeight = itemHeight + 0.02f; // Start with the height of the first item

                if (firstRowItem.GetComponent<ProductInfo>().Category == "Can" || firstRowItem.GetComponent<ProductInfo>().Category == "Soup" || firstRowItem.GetComponent<ProductInfo>().Category == "Noodles")
                
                    
                {
                    while (cumulativeHeight + itemHeight <= 0.27f)
                    {
                            Vector3 stackPosition = position + shelfComponent.transform.up * cumulativeHeight; // Adjust height for the stack
                            GameObject stackItem = Instantiate(firstRowItem, stackPosition, rotation);
                            stackItem.name = firstRowItem.name;
                            stackItem.tag = "Grippable";
                            stackItem.transform.SetParent(shelfComponent.transform);

                            // Turn off barcode and expiration date decals for the stack
                            ProductInteractionHandler stackInteractionHandler = stackItem.AddComponent<ProductInteractionHandler>();
                            Transform stackBarcodePlane = stackItem.transform.Find("Barcode");
                            DecalProjector stackExpirationDateDecal = stackItem.transform.Find("Expiration Date")?.GetComponent<DecalProjector>();
                            if (stackExpirationDateDecal == null)
                            {
                                stackExpirationDateDecal = stackItem.GetComponentInChildren<DecalProjector>();
                                Debug.Log("Stack Child: " + stackExpirationDateDecal);
                            }
                            string stackRandomDate = System.DateTime.Now.AddDays(Random.Range(1, 365)).ToString("MM/dd/yyyy");

                            // expiration.SetExpirationDate(stackExpirationDateDecal, "Best Before " + stackRandomDate);

                            if (stackBarcodePlane != null)
                            {
                                stackInteractionHandler.BarcodePlane = stackBarcodePlane.gameObject;
                                stackBarcodePlane.gameObject.SetActive(false);
                            }
                            if (stackExpirationDateDecal != null)
                            {
                                stackInteractionHandler.ExpirationDateDecal = stackExpirationDateDecal.gameObject;
                                stackExpirationDateDecal.gameObject.SetActive(false);
                            }

                            cumulativeHeight += itemHeight + 0.005f; // Increment cumulative height with a small buffer
                            Debug.Log($"Placed stack of {firstRowItem.name} at position: {stackPosition}");
                        }

                }
            }
        }
    }

    public void CombineMeshes(string ShelfId)
    {
        GameObject shelf = shelves[ShelfId];
        MeshFilter[] meshFilters = shelf.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        MeshFilter shelfMeshFilter = shelf.AddComponent<MeshFilter>();
        MeshRenderer shelfMeshRenderer = shelf.AddComponent<MeshRenderer>();
        shelfMeshFilter.mesh = combinedMesh;
        shelfMeshRenderer.materials = meshFilters[0].GetComponent<MeshRenderer>().materials;
    }

    public void SpawnPriceTag(string ShelfId, string ProductName, float Price, string Unit, float CenterX, float CenterY, float CenterZ)    
    {
        GameObject PriceTagInstance = Instantiate(PriceTag);
        PriceTag PriceTagComponent = PriceTagInstance.AddComponent<PriceTag>();
        PriceTagComponent.ProductName = ProductName;
        PriceTagComponent.Price = Price;
        PriceTagComponent.Unit = Unit;
        TMPro.TextMeshProUGUI productNameText = PriceTagInstance.transform.Find("Canvas/Product Name").GetComponent<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI priceText = PriceTagInstance.transform.Find("Canvas/Price").GetComponent<TMPro.TextMeshProUGUI>();

        if (productNameText != null)
        {
            productNameText.text = ProductName;
        }
        else
        {
            Debug.LogError("ProductNameText component not found on PriceTagInstance.");
        }

        if (priceText != null)
        {
            priceText.text = Price.ToString("F2") + " / " + Unit;
        }
        else
        {
            Debug.LogError("PriceText component not found on PriceTagInstance.");
        }
        PriceTagInstance.transform.SetParent(shelves[ShelfId].transform);
        PriceTagInstance.transform.localPosition = new Vector3(CenterX, CenterY, CenterZ - 0.001f);
        PriceTagInstance.transform.rotation = shelves[ShelfId].transform.rotation;
    }

    public void RandomizePrices(int seed)
    {
        Random.InitState(seed); // Set a fixed seed for reproducibility
        foreach (var category in groceryData.Categories)
        {
            foreach (var item in category.Items)
            {
                // Check if the item already exists in the dictionary
                if (!priceDictionary.ContainsKey(item))
                {
                    float randomPrice = Random.Range(1.0f, 500.0f); // Generate a random price between 1 and 500
                    priceDictionary.Add(item, randomPrice);
                }
            }
        }
    }

    // Method to load prices from the CSV file
    public void LoadPricesFromCSV()
    {
        // Load the CSV file from the Resources folder
        TextAsset csvFile = Resources.Load<TextAsset>("ground_truth"); // Ensure the file is in a Resources folder
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Resources folder!");
            return;
        }

        // Parse the CSV file
        string[] lines = csvFile.text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

            string[] values = line.Split(',');
            if (values.Length < 5) continue; // Ensure the line has at least 5 columns (ProductName, PesoPrice, DollarPrice, etc.)

            string productName = values[1].Trim(); // Column 1: Product Name
            float pesoPrice = 0f; // Default to 0
            float dollarPrice = 0f; // Default to 0

            // Try to parse Peso price
            if (!string.IsNullOrWhiteSpace(values[3]) && float.TryParse(values[3].Trim(), out float parsedPesoPrice))
            {
                pesoPrice = parsedPesoPrice;
            }

            // Try to parse Dollar price
            if (!string.IsNullOrWhiteSpace(values[4]) && float.TryParse(values[4].Trim(), out float parsedDollarPrice))
            {
                dollarPrice = parsedDollarPrice;
            }

            // Add the price to the dictionary based on the selected price unit
            if (selectedPriceUnit == PriceUnit.Peso)
            {
                priceDictionary[productName] = pesoPrice;
            }
            else if (selectedPriceUnit == PriceUnit.Dollar)
            {
                priceDictionary[productName] = dollarPrice;
            }
        }

        Debug.Log("Prices loaded successfully!");
    }

    public void ResetEnvironment()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void SwitchEnvironment(string sceneName)
    {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("Scene " + sceneName + " does not exist!");
            }
    }
    
    [System.Serializable]
    public class Products
    {
        public string Category;
        public List<string> Items;
    }
}