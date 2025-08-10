import { useState, useEffect } from 'react'
import ProductCart from './shared/ProductCart'

const Products = () => {
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [alert, setAlert] = useState("");

    const [searchKey, setSearchKey] = useState("");
    const [category, setCategory] = useState("");

    const baseUrl = import.meta.env.VITE_API_BASE_URL;

    useEffect(() => {

        const fetchProducts = async () => {
            try{       
                
                const baseApi = `${baseUrl}/Home/products`;
                const params = new URLSearchParams();
      
                if(searchKey) params.append("searchKey", searchKey);
                if(category) params.append("category", category);

                const apiUrl = params.toString() ? `${baseApi}?${params.toString()}` : baseApi;

                const response = await fetch(apiUrl);

                if(!response.ok) setAlert("Failed to fetch products");

                else {
                    const data = await response.json();
                    
                    if(Array.isArray(data) && data.length > 0){
                        setAlert("");
                        setProducts(data);
                    } 
                    else setAlert("No Records Found");
                }
            }
            catch(err){
               console.error(err); 
               setAlert(err);
            }
            finally{
                setLoading(false)
            }
        }

        const fetchCategories = async () => {
            try{
                const response = await fetch(`${baseUrl}/Home/Categories`);
                
                if(!response.ok)
                    throw new Error("Failed to fetch products");

                const data = await response.json();
                setCategories(data);
            }
            catch(err){
               console.error(err); 
            }
        }

        fetchProducts();
        fetchCategories();
    }, [searchKey, category]);

    
    return (
        <>
            <div className='container mx-auto py-15'>
                <div className='mb-6 grid grid-cols-1 sm:grid-cols-2 gap-2'>
                    <div>
                        <label for="countries" class="block mb-2 text-sm font-medium text-gray-900 ">Search Product</label>
                        <input type="text" id="product_search" class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5" placeholder="Search products..." value={searchKey} onChange={(e) => setSearchKey(e.target.value)}/>
                    </div>
                    <div>
                        <label for="countries" class="block mb-2 text-sm font-medium text-gray-900 ">Select Category</label>
                        <select value={category} onChange={(e) => setCategory(e.target.value)} id="countries" class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5">
                            <option value="" selected>Choose a category</option>
                            {categories.map((category, index) => <option key={index} value={category}>{category}</option>)}
                        </select>
                    </div>
                </div>
                {loading
                        ? (<p>Loading... Please wait...</p>)
                        : (!alert
                            ? (<div className='grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3'>
                                {products.map((product, index) => (
                                    <ProductCart key={index} {...product}/>
                                ))}        
                            </div>)
                            : (
                                <div class="p-4 mb-4 text-sm text-red-800 rounded-lg bg-red-50 w-full" role="alert">
                                    <span class="font-medium">{alert}</span>
                                </div>
                            )
                        )
                    }               
            </div>
        </>
    )
}

export default Products