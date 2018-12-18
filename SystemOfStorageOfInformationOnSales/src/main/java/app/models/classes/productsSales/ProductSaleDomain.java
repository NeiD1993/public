package app.models.classes.productsSales;

import app.models.IDomain;

import javax.persistence.*;
import javax.persistence.Entity;
import javax.persistence.Table;

/**
 * Created by NeiD on 22.11.2016.
 */
@Entity
@Table(name = "PRODUCT_SALE")
public class ProductSaleDomain implements IDomain {

    @Id
    @Column(name = "ID", updatable = false)
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    @ManyToOne
    @JoinColumn(name = "PRODUCT_NAME", nullable = false, updatable = false)
    private ProductDomain product;

    @ManyToOne
    @JoinColumn(name = "SALE_ID", nullable = false, updatable = false)
    private SaleDomain sale;

    @Column(name = "COUNT", nullable = false, updatable = false)
    private Integer count;

    @Column(name = "COST", nullable = false, updatable = false)
    private Float cost;

    @Column(name = "DISCOUNT", nullable = false, updatable = false)
    private Float discount;

    public ProductDomain getProduct() {
        return product;
    }

    public void setProduct(ProductDomain product) {
        this.product = product;
    }

    public SaleDomain getSale() {
        return sale;
    }

    public void setSale(SaleDomain sale) {
        this.sale = sale;
    }

    public Integer getCount() {
        return count;
    }

    public void setCount(Integer count) {
        this.count = count;
    }

    public Float getCost() {
        return cost;
    }

    public void setCost(Float cost) {
        this.cost = cost;
    }

    public Float getDiscount() {
        return discount;
    }

    public void setDiscount(Float discount) {
        this.discount = discount;
    }

}
