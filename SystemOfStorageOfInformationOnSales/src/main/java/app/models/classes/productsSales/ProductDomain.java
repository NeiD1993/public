package app.models.classes.productsSales;

import app.models.IDomain;

import javax.persistence.*;
import javax.persistence.Entity;
import javax.persistence.Table;
import java.util.List;

/**
 * Created by NeiD on 21.11.2016.
 */
@Entity
@Table(name = "PRODUCTS")
public class ProductDomain implements IDomain {

    @Id
    @Column(name = "NAME", nullable = false, updatable = false)
    private String name;

    @Column(name = "PRICE", nullable = false)
    private Float price;

    @OneToMany(mappedBy = "product")
    private List<ProductSaleDomain> sales;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        if(name.length() > 0)
            this.name = name;
    }

    public Float getPrice() {
        return price;
    }

    public void setPrice(Float price) {
        if(price > 0)
            this.price = !price.equals(this.price) ? price : null;
    }

    public List<ProductSaleDomain> getSales() {
        return sales;
    }

    public void setSales(List<ProductSaleDomain> sales) {
        this.sales = sales;
    }

}
