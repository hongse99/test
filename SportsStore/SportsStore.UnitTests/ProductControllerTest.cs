using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SportsStore.Domain.Abstract;
using Moq;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;



namespace SportsStore.UnitTests
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] { 
                new Product { ProductID=1, Name="P1"},
                new Product { ProductID=2, Name="P2"},
                new Product { ProductID=3, Name="P3"},
                new Product { ProductID=4, Name="P4"},
                new Product { ProductID=5, Name="P5"}
            }.AsQueryable());

            //컨트롤러를 생성하고 페이지 크기를 3개로 지정한다.
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Action
            IEnumerable<Product> result = (IEnumerable<Product>)controller.List(null, 2).Model;

            //Assert
            Product[] prodArray = result.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links() { 
            //Arrange - HTML 도우미 정의한다.     
            //확장메서드를 적용하는데 필요한다.
            HtmlHelper myHelper = null;
            //Arrange - PagingInfo 데이타를 생성한다.
            PagingInfo pagingInfo = new PagingInfo { 
                CurrentPage =2,
                TotalItems =28,
                ItemsPerPage = 10
            };

            //Arrange - 람다 표현식으로 델리게이트를 설정한다.
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //Assert
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a><a class=""selected"" href=""Page2"">2</a><a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model() { 
            //Arrange
            //- Mock 리파지터리를 생성한다 
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID =1, Name="p1"},
                new Product { ProductID =2, Name="p2"},
                new Product { ProductID =3, Name="p3"},
                new Product { ProductID =4, Name="p4"},
                new Product { ProductID =5, Name="p5"},
            }.AsQueryable());

            //Arrange - 컨트롤러를 생성하고 페이지 크기를 3개로 만든다
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Action
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
            
            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "p4");
            Assert.AreEqual(prodArray[1].Name, "p5");

        }

        [TestMethod]
        public void Can_Filter_Products() { 
            //Arrange
            //-Mock 레파지터리를 만든다
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[] { 
                    new Product { ProductID=1, Name="P1", Category="Cat1"},
                    new Product { ProductID=2, Name="P2", Category="Cat2"},
                    new Product { ProductID=3, Name="P3", Category="Cat1"},
                    new Product { ProductID=4, Name="P4", Category="Cat2"},
                    new Product { ProductID=5, Name="P5", Category="Cat3"}
                }.AsQueryable()
            );

            // Arrange - 컨트롤러를 생성하고 페이지 크기를 3으로 지정한다
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Action
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //Arrange
            //- Mock 리파지터리를 생성한다
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns( new Product[] { 
                    new Product {ProductID=1, Name="P1", Category="Apples"},
                    new Product {ProductID=2, Name="P2", Category="Apples"},
                    new Product {ProductID=3, Name="P3", Category="Plums"},
                    new Product {ProductID=4, Name="P4", Category="Oranges"}
                }.AsQueryable() );

            //Arrange - 컨트롤러를 생성한다 
            NaviController target = new NaviController(mock.Object);

            //Act  - 카테고리 목록을 가져온다
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //Arrange - mock 레파지터리를 생성한다
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] { 
                new Product {ProductID=1, Name="P1", Category="Apples"},
                new Product {ProductID=4, Name="P2", Category="Oranges"}
            }.AsQueryable());

             //Arrange - 컨트롤러를 생성한다.
            NaviController target = new NaviController(mock.Object);

            //Arrange  - 선택된 카테고리를 정의한다
            string categoryToSelect = "Apples";

            //Action
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //Assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //Arrange - Mock 리파지터리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] { 
                new Product{ ProductID=1, Name="P1", Category="Cat1"} ,
                new Product{ ProductID=2, Name="P2", Category="Cat2"} ,
                new Product{ ProductID=3, Name="P3", Category="Cat1"} ,
                new Product{ ProductID=4, Name="P4", Category="Cat2"} ,
                new Product{ ProductID=5, Name="P5", Category="Cat3"} 
            }.AsQueryable());

            //Arrange - 컨트롤러를 생성하고 페이지 크기를 3으로 지정한다.
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            //Action - 카테고리 별로 상품갯수를 테스트한다.
            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            //Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }

        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //Arrange - 테스트 상품 몇개를 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange - 새 장바구니를 생성한다.
            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            //Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines() { 
            //Arrange - 테스트 상품을 몇개 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange  - 새바구니를 생성한다.
            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);

            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line() { 
            //Arrange - 테스트 상품을 몇개 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            //Arrange - 새 장바구니를 만든다.
            Cart target = new Cart();

            //장바구니에 상품 몇개를 넣는다
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            //Act
            target.RemoveLine(p2);

            //Assert
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);

        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //Arrange  - 테스트 상품을 몇개 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };
            //Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };

            //Arrange - 새 장바구니를 생성한다.
            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            //Assert
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //Arrange  - 테스트 상품을 몇개 생성한다.
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //Arrange - 새 장바구니를 만든다
            Cart target = new Cart();

            //Arrange - 장바구니에 상품 몇개를 담는다.
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            //Act  장바구니를 초기화한다.
            target.Clear();

            //Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //Arrange - Mock 리파지터리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID =1 , Name ="P1", Category="Apples" }
            }.AsQueryable());

            //Arrange - CART 를 생성한다.
            Cart cart = new Cart();

            //Arrange  컨트롤러를 생성한다.
            CartController target = new CartController(mock.Object);

            //Act - 카트에 상품하나 추가
            target.AddToCart(cart, 1, null);

            //Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen() {
            //Arrange - Mock 리파지터리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID =1 , Name ="P1", Category="Apples" }
            }.AsQueryable());

            //Arrange - CART 를 생성한다.
            Cart cart = new Cart();

            //Arrange  컨트롤러를 생성한다.
            CartController target = new CartController(mock.Object);

            //Act - 카트에 상품하나 추가
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            //Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");

        }

        [TestMethod]
        public void Can_View_Cart_Content()
        {
            //Arrange - 카트를 생성한다.
            Cart cart = new Cart();

            //Arrange - 컨트롤러를 생성한다.
            CartController target = new CartController(null);

            //Act - Index 액션메서드를 호출한다.
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            //Assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
    }
}
