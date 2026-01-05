/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import SectionHeading from '../section-heading'
import ItemsCarousel from '../items-carousel'

const Container = styled.div`
  & .padding-mitigator {
    margin-top: 1.5rem;
    margin-right: -3rem;
  }
`

const ProductsCarousel = ({ children, title, to }) => {
  return (
    <Container>
      <SectionHeading title={title} to={to} />
      <div className="padding-mitigator">
        <ItemsCarousel>{children}</ItemsCarousel>
      </div>
    </Container>
  )
}

export default ProductsCarousel
