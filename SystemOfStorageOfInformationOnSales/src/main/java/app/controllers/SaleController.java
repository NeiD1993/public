package app.controllers;

import app.controllers.base.BaseDomainController;
import app.models.classes.BeginDateEndDates.DiscountStoryDomain;
import app.models.classes.productsSales.ProductDomain;
import app.models.classes.productsSales.ProductSaleDomain;
import app.models.classes.productsSales.SaleDomain;
import app.models.classes.factory.ProductSaleDomainFactory;
import app.models.classes.wrapper.ProductSaleListWrapper;
import app.services.DiscountStoryService;
import app.services.ProductService;
import app.services.SaleService;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.servlet.ModelAndView;

import javax.annotation.Resource;
import java.util.*;

/**
 * Created by NeiD on 26.11.2016.
 */
@Controller
@RequestMapping("/sales")
public class SaleController extends BaseDomainController<Integer, SaleDomain> {

    @Resource(name = "productService")
    private ProductService productService;

    @Resource(name = "saleService")
    private SaleService saleService;

    @Resource(name = "discountStoryService")
    private DiscountStoryService discountStoryService;

    @RequestMapping(method = RequestMethod.GET)
    public ModelAndView getAllSales() {
        return getAllDomains("sales/allSalesView", "sales", saleService);
    }

    @RequestMapping(value = "/add", method = RequestMethod.GET)
    public ModelAndView getAddSale(@RequestParam("positionCount") Integer positionCount) {
        logger.debug("Received request to show add sale page");
        ModelAndView modelAndView;
        List<String> productNames = productService.getAllProductNames();
        if (productNames.isEmpty())
            modelAndView = new ModelAndView("products/allProductsView", "products", productNames);
        else {
            modelAndView = new ModelAndView("sales/addSaleView");
            modelAndView.addObject("productSalesWrapper", new ProductSaleListWrapper(ProductSaleDomainFactory.initializeProductSales(positionCount)));
            modelAndView.addObject("productsNames", productNames);
        }
        return modelAndView;
    }

    @RequestMapping(value = "/add/confirm", method = RequestMethod.POST)
    public ModelAndView getAddSale(@ModelAttribute("productSalesWrapper") ProductSaleListWrapper productSalesWrapper) {
        logger.debug("Received request to show add/confirm sale page");
        Byte currentDiscountValue = null;
        Integer productCount;
        Float cost = 0f, discount = 0f, discountProductPrice, productsCost, productDiscount;
        SaleDomain sale = new SaleDomain();
        ProductDomain product, currentDiscountProduct = null;
        List<ProductSaleDomain> productSales = productSalesWrapper.getProductSales();
        DiscountStoryDomain currentDiscount = discountStoryService.getCurrentDiscount();
        if (currentDiscount != null) {
            currentDiscountProduct = currentDiscount.getProduct();
            currentDiscountValue = currentDiscount.getDiscount();
        }
        for (ProductSaleDomain productSale : productSales) {
            product = productSale.getProduct();
            productCount = productSale.getCount();
            if (currentDiscountProduct != null && productSale.getProduct().getName().equals(currentDiscountProduct.getName())) {
                discountProductPrice = currentDiscountProduct.getPrice();
                product.setPrice(discountProductPrice);
                productDiscount = (currentDiscountValue * discountProductPrice * productCount) / 100;
                productSale.setDiscount(productDiscount);
                discount += productDiscount;
            }
            else {
                product.setPrice(productService.getProductPrice(product.getName()));
                productSale.setDiscount(0f);
            }
            productsCost = product.getPrice() * productCount;
            productSale.setCost(productsCost);
            cost += productsCost;
        }
        sale.setCost(cost);
        sale.setDiscount(discount);
        sale.setProducts(productSales);
        ModelAndView modelAndView = new ModelAndView("sales/addSaleViewConfirm", "sale", sale);
        modelAndView.addObject("saleCost", cost);
        modelAndView.addObject("saleDiscount", discount);
        return modelAndView;
    }

    @RequestMapping(value = "/add", method = RequestMethod.POST)
    public String postAddSale(@ModelAttribute("sale") SaleDomain sale) {
        sale.setDate(new Date(System.currentTimeMillis()));
        List<ProductSaleDomain> productSales = sale.getProducts();
        for (ProductSaleDomain productSale : productSales)
            productSale.setSale(sale);
        postAddDomain("sale", sale, saleService);
        return "redirect:/sales";
    }

    @RequestMapping(value = "/products", method = RequestMethod.GET)
    public ModelAndView getProductSales(@RequestParam("id") Integer id) {
        logger.debug("Received request to show sale products page");
        return new ModelAndView("/sales/saleProductsView", "sale", saleService.getWithProducts(id));
    }

}
