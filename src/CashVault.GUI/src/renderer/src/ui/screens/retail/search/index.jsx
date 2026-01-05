import styled from '@emotion/styled'
import ProductCardSearch from '@ui/components/cards/product-card-search'
import RetailContainer from '@ui/components/retail-container'
import SearchBar from '@ui/components/search-bar'

const Container = styled.div`
  padding-top: 2rem !important;
  margin-bottom: 1rem;

  & .products {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-top: 3rem;
  }
`

const items = [
  {
    title: 'Nike Sportswear Chill Knit',
    category: 'T-shirts',
    price: 29.99,
    image: 'https://placehold.co/600x400'
  },
  {
    title: 'Nike Sportswear Chill Knit',
    category: 'T-shirts',
    price: 29.99,
    image: 'https://placehold.co/600x400',
    discount: 0.5
  },
  {
    title: 'Nike Sportswear Chill Knit',
    category: 'T-shirts',
    price: 29.99,
    image: 'https://placehold.co/600x400',
    discount: 0.5
  },
  {
    title: 'Nike Sportswear Chill Knit',
    category: 'T-shirts',
    price: 29.99,
    image: 'https://placehold.co/600x400',
    discount: 0.5
  },
  {
    title: 'Nike Sportswear Chill Knit',
    category: 'T-shirts',
    price: 29.99,
    image: 'https://placehold.co/600x400',
    discount: 0.5
  }
]

const SearchScreen = () => {
  return (
    <RetailContainer>
      <Container>
        <SearchBar interactable={true} onChange={() => undefined} />

        <div className="products">
          {items.map((item, index) => (
            <ProductCardSearch
              key={index}
              title={item.title}
              image={item.image}
              subtitle={item.category}
              price={item.price}
              discount={item.discount ?? 0}
            />
          ))}
        </div>
      </Container>
    </RetailContainer>
  )
}

export default SearchScreen
