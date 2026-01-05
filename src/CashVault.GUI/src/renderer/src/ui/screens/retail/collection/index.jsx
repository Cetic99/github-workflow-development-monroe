import styled from '@emotion/styled'
import { useNavigate, useParams } from 'react-router-dom'
import RetailContainer from '@ui/components/retail-container'
import IdleBackground from '@ui/assets/images/retail/idle-background.png'
import ProductsCarousel from '@ui/components/products-carousel'
import ProductCard from '@ui/components/cards/product-card'

const Container = styled.div`
  padding-top: 2rem !important;
  display: flex;
  flex-direction: column;
  gap: 3rem 0;
  position: relative;

  margin-bottom: 1.5rem;

  height: 100%;

  & .header {
    width: 100%;
    height: 31.25rem;
    background-image: url(${IdleBackground});
    background-size: cover;
    background-position: center;
    object-fit: fill;

    display: flex;
    flex-direction: column;
    justify-content: flex-end;
    align-items: center;

    position: relative;

    border-radius: 1rem;

    & .heading {
      position: relative;
      text-align: center;
      z-index: 2;
      padding-bottom: 6rem;
      h1 {
        font-size: 4.375rem;
        line-height: 4.375rem;
        color: white;
        font-weight: 700;
        margin-bottom: 0.625rem;
      }

      h2 {
        font-size: 1.5rem;
        line-height: 2rem;
        color: white;
        font-weight: 400;
      }
    }

    & .fade {
      position: absolute;
      bottom: 0;
      height: 50%;
      width: 100%;
      background: linear-gradient(180deg, rgba(0, 0, 0, 0) 0%, rgba(0, 0, 0, 0.85) 100%);

      border-radius: 1rem;
    }
  }
`
const products = [
  {
    title: 'Nike Sportswear Chill Knit',
    price: 29.99
  },
  {
    title: 'Nike Sportswear Phoenix Fleece',
    price: 75.99,
    discount: 0.15
  },
  {
    title: 'Jordan Rare Air Mens T-Shirt',
    price: 29.99
  },
  {
    title: 'Jordan Rare Air Mens T-Shirt',
    price: 29.99
  },
  {
    title: 'Jordan Rare Air Mens T-Shirt',
    price: 29.99
  },
  {
    title: 'Jordan Rare Air Mens T-Shirt',
    price: 29.99
  },
  {
    title: 'Jordan Rare Air Mens T-Shirt',
    price: 29.99
  },
  {
    title: 'Jordan Brooklyn Puffer Jacket',
    price: 29.99
  }
]

const CollectionScreen = () => {
  const params = useParams()
  const navigate = useNavigate()
  const { title } = params

  const handleProductClick = (title) => {
    navigate(`/retail/products/${title}`)
  }

  return (
    <RetailContainer>
      <Container>
        <div className="header">
          <div className="heading">
            <h1>{decodeURI(title)}</h1>
            <h2>Find your game. Shop all things football.</h2>
          </div>
          <div className="fade"></div>
        </div>

        <ProductsCarousel title="Kits" to="/retail/back-to-school">
          {products.map((product, index) => (
            <div style={{ marginRight: '1.5rem' }} key={index}>
              <ProductCard
                title={product.title}
                subtitle={product.price}
                discount={product.discount ?? 0}
                fadeColor={product.discount ? '#004034' : '#000000'}
                onClick={() => handleProductClick(product.title)}
              />
            </div>
          ))}
        </ProductsCarousel>

        <ProductsCarousel title="Shoes" to="/retail/back-to-school">
          {products.map((product, index) => (
            <div style={{ marginRight: '1.5rem' }} key={index}>
              <ProductCard
                title={product.title}
                subtitle={product.price}
                discount={product.discount ?? 0}
                fadeColor={product.discount ? '#004034' : '#000000'}
                onClick={() => handleProductClick(product.title)}
              />
            </div>
          ))}
        </ProductsCarousel>

        <ProductsCarousel title="Accessories" to="/retail/back-to-school">
          {products.map((product, index) => (
            <div style={{ marginRight: '1.5rem' }} key={index}>
              <ProductCard
                title={product.title}
                subtitle={product.price}
                discount={product.discount ?? 0}
                fadeColor={product.discount ? '#004034' : '#000000'}
                onClick={() => handleProductClick(product.title)}
              />
            </div>
          ))}
        </ProductsCarousel>
      </Container>
    </RetailContainer>
  )
}

export default CollectionScreen
