import styled from '@emotion/styled'
import ProductCard from '@ui/components/cards/product-card'
import RetailContainer from '@ui/components/retail-container'
import { useLocation, useNavigate, useParams } from 'react-router-dom'
import Featured from '@ui/assets/images/retail/featured-background.png'
import RetailFilter from '@ui/components/retail-filter'

const Container = styled.div`
  padding-top: 2rem !important;
  position: relative;
  display: flex;
  flex-direction: column;
  margin-bottom: 1.5rem;
  gap: 3rem 0;

  height: 100%;

  & .header {
    & .cover-image {
      position: absolute;
      z-index: 0;
      top: -3rem;
      left: 0;
      background-image: url(${Featured});
      background-position: center;
      background-size: cover;
      object-fit: fill;
      height: 50vh;
      width: 100%;

      & .fade {
        position: absolute;
        bottom: 0;
        background: linear-gradient(180deg, rgba(15, 14, 24, 0) 0%, #0f0e18 100%);
        height: 100%;
        width: 100%;
      }
    }
  }

  & .header-text {
    position: relative;
    z-index: 2;
    display: flex;
    align-items: center;
    flex-direction: column;

    color: ${(p) => (p.themed === true ? 'white' : 'initial')};
    h1 {
      font-weight: 700;
      font-size: 4rem;
      line-height: 4.5rem;
      margin-bottom: 0.5;
    }

    h2 {
      font-weight: 400;
      font-size: 1.5rem;
      line-height: 2rem;
      margin-bottom: 1.5rem;
    }
  }

  & .products {
    display: flex;
    flex-wrap: wrap;
    gap: 1.5rem;

    > * {
      flex: 1 1 calc((100% / 3) - 1rem);
      max-width: calc((100% / 3) - 1rem);
    }

    @media (max-width: 800px) {
      > * {
        flex: 1 1 calc((100% / 2) - 1rem);
        max-width: calc((100% / 2) - 1rem);
      }
    }

    @media (max-width: 620px) {
      > * {
        flex: 1 1 100%;
        max-width: 100%;
      }
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

const CategoryScreen = () => {
  const params = useParams()
  const location = useLocation()
  const navigate = useNavigate()
  const { title } = params

  const queryParams = new URLSearchParams(location.search)
  const themed = queryParams.get('themed') === 'true'

  const handleProductClick = (title) => {
    navigate(`/retail/products/${title}`)
  }

  return (
    <RetailContainer color={themed === true ? 'dark' : 'light'}>
      <Container themed={themed}>
        {themed && (
          <div className="header">
            <div className="cover-image">
              <div className="fade"></div>
            </div>
          </div>
        )}

        <div className="header-text">
          <h1>{decodeURI(title)}</h1>
          <h2>{products.length} items</h2>
          <RetailFilter onApplyFilters={(results) => console.log(results)} />
        </div>

        <div className="products">
          {products.map((product, index) => (
            <ProductCard
              key={product.title + index}
              title={product.title}
              subtitle={product.price}
              discount={product.discount ?? 0}
              fadeColor={product.discount ? '#004034' : '#000000'}
              onClick={() => handleProductClick(product.title)}
            />
          ))}
        </div>
      </Container>
    </RetailContainer>
  )
}

export default CategoryScreen
