package app.controllers;

import app.controllers.base.BaseDomainGetAddedWithoutAttributesController;
import app.models.classes.productsSales.ProductDomain;
import app.services.ProductService;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.servlet.ModelAndView;

import javax.annotation.Resource;

/**
 * Created by NeiD on 26.11.2016.
 */
@Controller
@RequestMapping("/products")
public class ProductController extends BaseDomainGetAddedWithoutAttributesController<String, ProductDomain> {

    @Resource(name = "productService")
    private ProductService productService;

    @RequestMapping(method = RequestMethod.GET)
    public ModelAndView getAllProducts() {
        return getAllDomains("products/allProductsView", "products", productService);
    }

    @RequestMapping(value = "/add", method = RequestMethod.GET)
    public ModelAndView getAddProduct() {
        return getAddDomain("products/addProductView", "product", new ProductDomain());
    }

    @RequestMapping(value = "/add", method = RequestMethod.POST)
    public String postAddProduct(@ModelAttribute("product") ProductDomain product) {
        postAddDomain("product", product, productService);
        return "redirect:/products";
    }

    @RequestMapping(value = "/edit", method = RequestMethod.GET)
    public ModelAndView getEditProduct(@RequestParam("name") String name) {
        logger.debug("Received request to show edit product page");
        return new ModelAndView("/products/editProductView", "product", productService.get(name));
    }

    @RequestMapping(value = "/edit", method = RequestMethod.POST)
    public String postEditProduct(@RequestParam("name") String name, @ModelAttribute("product") ProductDomain product) {
        logger.debug("Received request to edit existing product");
        product.setName(name);
        productService.edit(product);
        return "redirect:/products";
    }

    @RequestMapping(value = "/sales", method = RequestMethod.GET)
    public ModelAndView getProductSales(@RequestParam("name") String name, @RequestParam("wRP") Boolean wRP) {
        logger.debug("Received request to show product sales page");
        ModelAndView modelAndView = new ModelAndView("/products/productSalesView");
        if(!wRP) {
            modelAndView.addObject("sales", productService.getSales(name));
            modelAndView.addObject("wRP", false);
        }
        else {
            modelAndView.addObject("sales", productService.getSalesWRP(name));
            modelAndView.addObject("wRP", true);
        }
        return modelAndView;
    }

}
