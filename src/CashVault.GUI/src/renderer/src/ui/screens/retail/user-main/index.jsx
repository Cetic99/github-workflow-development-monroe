/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import ItemsCarousel from '@ui/components/items-carousel'
import CategoryCarouselItem from '@ui/components/category-carousel-item'
import RetailContainer from '@ui/components/retail-container'
import SearchBar from '@ui/components/search-bar'
import FeaturedCard from '@ui/components/cards/featured-card'
import ProductsCarousel from '@ui/components/products-carousel'
import ProductCard from '@ui/components/cards/product-card'
import CollectionCard from '@ui/components/cards/collection-card'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  padding-top: 2rem !important;
  display: flex;
  flex-direction: column;
  margin-bottom: 1.5rem;
  gap: 3rem 0;

  height: 100%;

  & .padding-mitigator {
    margin-right: -3rem;
  }
`

const carouselCategories = [
  {
    title: 'Summer Sale',
    image: 'https://placehold.co/600x400',
    to: '/retail/categories/Summer%20Sale?themed=true'
  },
  {
    title: 'T-Shirts',
    image: 'https://placehold.co/600x400',
    to: '/retail/categories/T-Shirts'
  },
  {
    title: 'Hoodies',
    image: 'https://placehold.co/600x400',
    to: '/retail/categories/Hoodies'
  },
  {
    title: 'Jackets',
    image: 'https://placehold.co/600x400',
    to: '/retail/categories/Jackets'
  },
  {
    title: 'Tracksuits',
    image: 'https://placehold.co/600x400',
    to: '/retail/categories/Tracksuits'
  },
  {
    title: 'Shoes',
    image: 'https://placehold.co/600x400',
    to: '/retail/categories/Shoes'
  },
  {
    title: 'Leggings',
    image: 'https://placehold.co/600x400',
    to: '/retail/categories/Leggings'
  }
]

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
    title: 'Jordan Brooklyn Puffer Jacket',
    price: 29.99
  }
]

const collections = [
  { title: '⚽️ Football', to: '/retail/collections/Football' },
  { title: '🏃‍♀️ Running', to: '/retail/collections/Football' },
  { title: '🏋 Fitness', to: '/retail/collections/Football' }
]

const UserMainScreen = () => {
  const navigate = useNavigate()

  const handleSearchClick = () => {
    navigate('/retail/search')
  }

  const handleProductClick = (title) => {
    navigate(`/retail/products/${title}`)
  }

  return (
    <RetailContainer>
      <Container>
        <SearchBar onClick={handleSearchClick} />

        <div className="padding-mitigator">
          <ItemsCarousel>
            {carouselCategories.map((category, index) => (
              <CategoryCarouselItem
                key={index}
                title={category.title}
                image={category.image}
                to={category.to}
              />
            ))}
          </ItemsCarousel>
        </div>

        <FeaturedCard
          title="Fitness Madness Sale"
          subtitle="Get into shape, no more excuses"
          badgeText="Up to 75% Off"
          topRightText="🔥 ENDS IN 20:48:39"
          to="/retail/categories/Fitness%20Madness%20Sale?themed=true"
        />

        <ProductsCarousel title="Featured products" to="/retail/featured-products">
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

        <ProductsCarousel title="Collections" to="/retail/collections">
          {collections.map((collection, index) => (
            <div style={{ marginRight: '1.5rem' }} key={index}>
              <CollectionCard title={collection.title} to={collection.to} />
            </div>
          ))}
        </ProductsCarousel>

        <ProductsCarousel title="Best sellers" to="/retail/best-sellers">
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

        <ProductsCarousel title="Back to school" to="/retail/back-to-school">
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

export default UserMainScreen
